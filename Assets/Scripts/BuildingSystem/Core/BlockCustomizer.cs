using System;
using UnityEngine;

public class BlockCustomizer : MonoBehaviour
{
    public static BlockCustomizer Instance { get; private set; }

    [Header("颜色选择器")]
    public ColorPickerUI ColorPicker;

    private BlockController _selectedBlock;

    public event Action<BlockController> OnBlockSelected;
    public event Action<BlockController, Color> OnBlockColorChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectBlock(BlockController block)
    {
        _selectedBlock = block;
        OnBlockSelected?.Invoke(block);

        if (ColorPicker != null)
        {
            ColorPicker.Show(block.GetColor());
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