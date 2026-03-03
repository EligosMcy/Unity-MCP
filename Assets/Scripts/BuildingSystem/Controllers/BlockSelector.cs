using System;
using System.Collections;
using UnityEngine;

public class BlockSelector : MonoBehaviour
{
    public event Action<BlockController> OnBlockHoverEnter;
    public event Action<BlockController> OnBlockHoverExit;
    public event Action<BlockController> OnBlockClick;

    [Header("GizmoCube预制体")]
    [SerializeField]
    private GameObject _gizmoCubePrefab;

    [Header("颜色设置")]
    [SerializeField]
    private Color _hoverColor = Color.yellow;
    [SerializeField]
    private Color _selectColor = Color.green;

    [Header("大小设置")]
    [SerializeField]
    private float _hoverScale = 1.05f;
    [SerializeField]
    private float _selectScale = 1.08f;

    [Header("检测设置")]
    [SerializeField]
    private float _raycastInterval = 0.05f;

    private BlockController _hoveredBlock;
    private BlockController _selectedBlock;
    private Camera _mainCamera;
    private BuildingInputProvider _inputProvider;
    private Coroutine _raycastCoroutine;

    private GameObject _hoverGizmoCubeInstance;
    private GameObject _selectGizmoCubeInstance;

    public bool IsColorMode { get; set; }

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
            _inputProvider.OnMouseClicked += handleMouseClick;
        }

        initializeGizmoCubes();
        _raycastCoroutine = StartCoroutine(raycastLoop());
    }

    private IEnumerator raycastLoop()
    {
        while (true)
        {
            if (IsColorMode && _mainCamera != null && _inputProvider != null)
            {
                handleRaycast();
            }
            yield return new WaitForSeconds(_raycastInterval);
        }
    }

    private void initializeGizmoCubes()
    {
        if (_gizmoCubePrefab != null)
        {
            _hoverGizmoCubeInstance = Instantiate(_gizmoCubePrefab, Vector3.zero, Quaternion.identity);
            _hoverGizmoCubeInstance.name = "HoverGizmoCube";
            _hoverGizmoCubeInstance.SetActive(false);

            _selectGizmoCubeInstance = Instantiate(_gizmoCubePrefab, Vector3.zero, Quaternion.identity);
            _selectGizmoCubeInstance.name = "SelectGizmoCube";
            _selectGizmoCubeInstance.SetActive(false);

            setGizmoCubeColor(_hoverGizmoCubeInstance, _hoverColor);
            setGizmoCubeColor(_selectGizmoCubeInstance, _selectColor);
        }
    }

    private void setGizmoCubeColor(GameObject gizmoCube, Color color)
    {
        if (gizmoCube == null)
            return;

        Renderer[] renderers = gizmoCube.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    private void OnDestroy()
    {
        if (_raycastCoroutine != null)
        {
            StopCoroutine(_raycastCoroutine);
        }

        if (_inputProvider != null)
        {
            _inputProvider.OnMouseClicked -= handleMouseClick;
        }

        if (_hoverGizmoCubeInstance != null)
        {
            Destroy(_hoverGizmoCubeInstance);
        }

        if (_selectGizmoCubeInstance != null)
        {
            Destroy(_selectGizmoCubeInstance);
        }
    }

    private void Update()
    {
    }

    private void handleRaycast()
    {
        if (!IsColorMode)
            return;

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
                    if (_hoveredBlock != null && _hoveredBlock != _selectedBlock)
                    {
                        _hoveredBlock.OnHoverExit();
                        OnBlockHoverExit?.Invoke(_hoveredBlock);
                    }

                    _hoveredBlock = block;

                    if (_hoveredBlock != _selectedBlock)
                    {
                        _hoveredBlock.OnHoverEnter();
                        OnBlockHoverEnter?.Invoke(_hoveredBlock);
                    }

                    updateHoverGizmoCube(_hoveredBlock);
                }
            }
            else
            {
                clearHover();
            }
        }
        else
        {
            clearHover();
        }

        updateGizmoCubeVisibility();
    }

    private void updateGizmoCubeVisibility()
    {
        if (_selectedBlock != null && _hoveredBlock == _selectedBlock)
        {
            hideHoverGizmoCube();
        }
    }

    private void handleMouseClick()
    {
        if (!IsColorMode)
            return;

        if (_hoveredBlock != null)
        {
            if (_selectedBlock != null && _selectedBlock != _hoveredBlock)
            {
                _selectedBlock.OnHoverExit();
                OnBlockHoverExit?.Invoke(_selectedBlock);
            }

            _selectedBlock = _hoveredBlock;
            _selectedBlock.OnHoverEnter();
            OnBlockHoverEnter?.Invoke(_selectedBlock);

            updateSelectGizmoCube(_selectedBlock);
            OnBlockClick?.Invoke(_selectedBlock);
        }
    }

    private void clearHover()
    {
        if (_hoveredBlock != null)
        {
            if (_hoveredBlock != _selectedBlock)
            {
                _hoveredBlock.OnHoverExit();
                OnBlockHoverExit?.Invoke(_hoveredBlock);
            }
            _hoveredBlock = null;
        }

        hideHoverGizmoCube();
    }

    private void updateHoverGizmoCube(BlockController targetBlock)
    {
        if (_hoverGizmoCubeInstance == null || targetBlock == null)
            return;

        if (_selectedBlock != null && targetBlock == _selectedBlock)
        {
            hideHoverGizmoCube();
            return;
        }

        showGizmoCube(_hoverGizmoCubeInstance, targetBlock, _hoverScale);
    }

    private void updateSelectGizmoCube(BlockController targetBlock)
    {
        if (_selectGizmoCubeInstance == null || targetBlock == null)
            return;

        showGizmoCube(_selectGizmoCubeInstance, targetBlock, _selectScale);
    }

    private void showGizmoCube(GameObject gizmoCube, BlockController targetBlock, float scale)
    {
        gizmoCube.transform.SetParent(targetBlock.transform);
        gizmoCube.transform.localPosition = Vector3.zero;
        gizmoCube.transform.localRotation = Quaternion.identity;
        gizmoCube.transform.localScale = Vector3.one * scale;
        gizmoCube.SetActive(true);
    }

    private void hideHoverGizmoCube()
    {
        if (_hoverGizmoCubeInstance != null)
        {
            _hoverGizmoCubeInstance.SetActive(false);
            _hoverGizmoCubeInstance.transform.SetParent(null);
        }
    }

    private void hideSelectGizmoCube()
    {
        if (_selectGizmoCubeInstance != null)
        {
            _selectGizmoCubeInstance.SetActive(false);
            _selectGizmoCubeInstance.transform.SetParent(null);
        }
    }

    public BlockController GetHoveredBlock()
    {
        return _hoveredBlock;
    }

    public BlockController GetSelectedBlock()
    {
        return _selectedBlock;
    }

    public void ClearSelection()
    {
        if (_hoveredBlock != null)
        {
            _hoveredBlock.OnHoverExit();
            OnBlockHoverExit?.Invoke(_hoveredBlock);
            _hoveredBlock = null;
        }

        _selectedBlock = null;
        hideHoverGizmoCube();
        hideSelectGizmoCube();
    }
}
