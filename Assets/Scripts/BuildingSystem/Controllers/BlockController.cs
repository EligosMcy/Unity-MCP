using UnityEngine;

public class BlockController : MonoBehaviour
{
    private BlockData _blockData;
    private Renderer _renderer;
    private bool _isHovered = false;
    private Color _originalColor;

    public BlockData BlockData => _blockData;

    public void Initialize(BlockData blockData)
    {
        _blockData = blockData;
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            SetColor(_blockData.Color);
        }
    }

    public void SetColor(Color newColor)
    {
        if (_renderer != null)
        {
            _renderer.material.color = newColor;
            _originalColor = newColor;
        }
    }

    public Color GetColor()
    {
        return _originalColor;
    }

    public void OnHoverEnter()
    {
        if (_renderer != null && !_isHovered)
        {
            _isHovered = true;
            _renderer.material.color = Color.Lerp(_originalColor, Color.white, 0.3f);
        }
    }

    public void OnHoverExit()
    {
        if (_renderer != null && _isHovered)
        {
            _isHovered = false;
            _renderer.material.color = _originalColor;
        }
    }
}
