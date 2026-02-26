using UnityEngine;

public class BlockController : MonoBehaviour
{
    private BlockData _blockData;
    private Renderer _renderer;

    public BlockData BlockData => _blockData;

    public void Initialize(BlockData blockData)
    {
        _blockData = blockData;
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            _renderer.material.color = blockData.color;
        }
    }

    public void SetColor(Color newColor)
    {
        if (_renderer != null)
        {
            _renderer.material.color = newColor;
            _blockData.color = newColor;
        }
    }

    public Color GetColor()
    {
        if (_renderer != null)
        {
            return _renderer.material.color;
        }
        return Color.white;
    }

    public void OnMouseEnter()
    {
        if (_renderer != null)
        {
            Color originalColor = _renderer.material.color;
            _renderer.material.color = Color.Lerp(originalColor, Color.white, 0.3f);
        }
    }

    public void OnMouseExit()
    {
        if (_renderer != null)
        {
            _renderer.material.color = _blockData.color;
        }
    }

    public void OnMouseDown()
    {
        BlockCustomizer.Instance?.SelectBlock(this);
    }
}