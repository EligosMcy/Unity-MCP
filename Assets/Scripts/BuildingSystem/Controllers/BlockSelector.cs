using System;
using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    public event Action<BlockController> OnBlockHoverEnter;
    public event Action<BlockController> OnBlockHoverExit;
    public event Action<BlockController> OnBlockClick;

    [SerializeField]
    private GameObject _gizmoCubePrefab;

    private BlockController _hoveredBlock;
    private Camera _mainCamera;
    private BuildingInputProvider _inputProvider;
    private GameObject _currentGizmoCubeInstance;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("BlockSelector: Main camera not found!");
        }

        _inputProvider = GetComponent<BuildingInputProvider>();
        if (_inputProvider != null)
        {
            _inputProvider.OnMouseClicked += HandleMouseClick;
        }
    }

    private void OnDestroy()
    {
        if (_inputProvider != null)
        {
            _inputProvider.OnMouseClicked -= HandleMouseClick;
        }
    }

    private void Update()
    {
        if (_mainCamera == null || _inputProvider == null)
            return;

        HandleRaycast();
    }

    private void HandleRaycast()
    {
        Vector2 mouseScreenPos = _inputProvider.GetMousePosition();
        Ray ray = _mainCamera.ScreenPointToRay(mouseScreenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            BlockController block = hit.collider.GetComponent<BlockController>();

            if (block != null)
            {
                if (_hoveredBlock != block)
                {
                    if (_hoveredBlock != null)
                    {
                        _hoveredBlock.OnHoverExit();
                        OnBlockHoverExit?.Invoke(_hoveredBlock);
                    }

                    _hoveredBlock = block;
                    _hoveredBlock.OnHoverEnter();
                    OnBlockHoverEnter?.Invoke(_hoveredBlock);
                    UpdateGizmoCube(_hoveredBlock);
                }
            }
            else
            {
                ClearHover();
            }
        }
        else
        {
            ClearHover();
        }
    }

    private void HandleMouseClick()
    {
        if (_hoveredBlock != null)
        {
            OnBlockClick?.Invoke(_hoveredBlock);
        }
    }

    private void ClearHover()
    {
        if (_hoveredBlock != null)
        {
            _hoveredBlock.OnHoverExit();
            OnBlockHoverExit?.Invoke(_hoveredBlock);
            _hoveredBlock = null;
        }
        ClearGizmoCube();
    }

    private void UpdateGizmoCube(BlockController targetBlock)
    {
        if (_gizmoCubePrefab == null || targetBlock == null)
            return;

        ClearGizmoCube();

        _currentGizmoCubeInstance = Instantiate(_gizmoCubePrefab, targetBlock.transform.position, Quaternion.identity);
        _currentGizmoCubeInstance.transform.SetParent(targetBlock.transform);
        _currentGizmoCubeInstance.transform.localPosition = Vector3.zero;
        _currentGizmoCubeInstance.transform.localRotation = Quaternion.identity;
        _currentGizmoCubeInstance.transform.localScale = Vector3.one * 1.05f;
    }

    private void ClearGizmoCube()
    {
        if (_currentGizmoCubeInstance != null)
        {
            Destroy(_currentGizmoCubeInstance);
            _currentGizmoCubeInstance = null;
        }
    }

    public BlockController GetHoveredBlock()
    {
        return _hoveredBlock;
    }

    public void ClearSelection()
    {
        ClearHover();
    }
}
