using System;
using UnityEngine;

public class BlockCustomizer : MonoBehaviour
{
    [Header("颜色选择器")]
    public ColorPickerUI ColorPicker;

    private BlockController _selectedBlock;
    private BlockSelector _blockSelector;

    public event Action<BlockController> OnBlockSelected;
    public event Action<BlockController, Color> OnBlockColorChanged;

    private void Start()
    {
        _blockSelector = GetComponent<BlockSelector>();
        if (_blockSelector != null)
        {
            _blockSelector.OnBlockClick += HandleBlockClick;
        }
        else
        {
            Debug.LogWarning("BlockCustomizer: BlockSelector component not found!");
        }
    }

    private void OnDestroy()
    {
        if (_blockSelector != null)
        {
            _blockSelector.OnBlockClick -= HandleBlockClick;
        }
    }

    private void HandleBlockClick(BlockController block)
    {
        SelectBlock(block);
    }

    public void SelectBlock(BlockController block)
    {
        _selectedBlock = block;
        OnBlockSelected?.Invoke(block);

        if (ColorPicker != null)
        {
            ColorPicker.SetInitialColor(block.GetColor());
        }
    }

    public void DeselectBlock()
    {
        _selectedBlock = null;

        if (ColorPicker != null)
        {
            ColorPicker.Hide();
        }
    }

    public void ApplyColor(Color newColor)
    {
        if (_selectedBlock != null)
        {
            _selectedBlock.SetColor(newColor);
            OnBlockColorChanged?.Invoke(_selectedBlock, newColor);
        }
    }

    public BlockController GetSelectedBlock()
    {
        return _selectedBlock;
    }

    public bool HasSelectedBlock()
    {
        return _selectedBlock != null;
    }
}
