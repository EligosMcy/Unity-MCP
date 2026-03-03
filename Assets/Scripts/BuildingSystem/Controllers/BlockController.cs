using UnityEngine;

public class BlockController : MonoBehaviour
{
    private const string BaseColor = "_BaseColor";

    private BlockData _blockData;
    private Renderer _renderer;
    private bool _isHovered = false;
    private Color _originalColor;
    private MaterialPropertyBlock _propertyBlock;

    public BlockData BlockData => _blockData;

    public void Initialize(BlockData blockData)
    {
        _blockData = blockData;
        _renderer = GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();

        if (_renderer != null)
        {
            SetColor(_blockData.Color);
        }
    }

    public void SetColor(Color newColor)
    {
        if (_renderer != null && _propertyBlock != null)
        {
            _propertyBlock.SetColor(BaseColor, newColor);
            _renderer.SetPropertyBlock(_propertyBlock);
            _originalColor = newColor;
        }
    }

    public Color GetColor()
    {
        return _originalColor;
    }

    public void OnHoverEnter()
    {
        if (_renderer != null && _propertyBlock != null && !_isHovered)
        {
            _isHovered = true;
            Color hoverColor = Color.Lerp(_originalColor, Color.white, 0.3f);
            _propertyBlock.SetColor(BaseColor, hoverColor);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    public void OnHoverExit()
    {
        if (_renderer != null && _propertyBlock != null && _isHovered)
        {
            _isHovered = false;
            _propertyBlock.SetColor(BaseColor, _originalColor);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
