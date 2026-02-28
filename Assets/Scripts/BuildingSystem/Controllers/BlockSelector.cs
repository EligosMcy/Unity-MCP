using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockSelector : MonoBehaviour
{
    public static BlockSelector Instance { get; private set; }

    public event Action<BlockController> OnBlockHoverEnter;
    public event Action<BlockController> OnBlockHoverExit;
    public event Action<BlockController> OnBlockClick;

    private BlockController _hoveredBlock;
    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("BlockSelector: Main camera not found!");
        }
    }

    private void Update()
    {
        if (_mainCamera == null)
            return;

        HandleRaycast();
    }

    private void HandleRaycast()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
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
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    OnBlockClick?.Invoke(_hoveredBlock);
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

    private void ClearHover()
    {
        if (_hoveredBlock != null)
        {
            _hoveredBlock.OnHoverExit();
            OnBlockHoverExit?.Invoke(_hoveredBlock);
            _hoveredBlock = null;
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
