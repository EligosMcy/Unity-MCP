using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorPickerUI : MonoBehaviour
{
    [Header("UI元素")]
    public GameObject Panel;
    public Image PreviewImage;
    public Slider RedSlider;
    public Slider GreenSlider;
    public Slider BlueSlider;
    public Button ApplyButton;
    public Button CancelButton;

    private Color _currentColor;

    public event UnityAction<Color> OnColorApplied;

    private void Awake()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);
        }

        if (RedSlider != null)
        {
            RedSlider.onValueChanged.AddListener(UpdateColor);
        }

        if (GreenSlider != null)
        {
            GreenSlider.onValueChanged.AddListener(UpdateColor);
        }

        if (BlueSlider != null)
        {
            BlueSlider.onValueChanged.AddListener(UpdateColor);
        }

        if (ApplyButton != null)
        {
            ApplyButton.onClick.AddListener(Apply);
        }

        if (CancelButton != null)
        {
            CancelButton.onClick.AddListener(Cancel);
        }
    }

    public void Show(Color initialColor)
    {
        _currentColor = initialColor;

        if (Panel != null)
        {
            Panel.SetActive(true);
        }

        UpdateSliders();
        UpdatePreview();
    }

    public void Hide()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);
        }
    }

    private void UpdateSliders()
    {
        if (RedSlider != null)
        {
            RedSlider.value = _currentColor.r;
        }

        if (GreenSlider != null)
        {
            GreenSlider.value = _currentColor.g;
        }

        if (BlueSlider != null)
        {
            BlueSlider.value = _currentColor.b;
        }
    }

    private void UpdateColor(float value)
    {
        if (RedSlider != null && GreenSlider != null && BlueSlider != null)
        {
            _currentColor = new Color(RedSlider.value, GreenSlider.value, BlueSlider.value);
            UpdatePreview();
        }
    }

    private void UpdatePreview()
    {
        if (PreviewImage != null)
        {
            PreviewImage.color = _currentColor;
        }
    }

    private void Apply()
    {
        OnColorApplied?.Invoke(_currentColor);
        Hide();
    }

    private void Cancel()
    {
        Hide();
    }
}