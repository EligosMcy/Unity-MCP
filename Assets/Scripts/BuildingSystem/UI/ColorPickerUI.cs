using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorPickerUI : MonoBehaviour
{
    [Header("UI元素")]
    public GameObject panel;
    public Image previewImage;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Button applyButton;
    public Button cancelButton;

    private Color _currentColor;

    public event UnityAction<Color> OnColorApplied;

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (redSlider != null)
        {
            redSlider.onValueChanged.AddListener(UpdateColor);
        }

        if (greenSlider != null)
        {
            greenSlider.onValueChanged.AddListener(UpdateColor);
        }

        if (blueSlider != null)
        {
            blueSlider.onValueChanged.AddListener(UpdateColor);
        }

        if (applyButton != null)
        {
            applyButton.onClick.AddListener(Apply);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(Cancel);
        }
    }

    public void Show(Color initialColor)
    {
        _currentColor = initialColor;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        UpdateSliders();
        UpdatePreview();
    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void UpdateSliders()
    {
        if (redSlider != null)
        {
            redSlider.value = _currentColor.r;
        }

        if (greenSlider != null)
        {
            greenSlider.value = _currentColor.g;
        }

        if (blueSlider != null)
        {
            blueSlider.value = _currentColor.b;
        }
    }

    private void UpdateColor(float value)
    {
        if (redSlider != null && greenSlider != null && blueSlider != null)
        {
            _currentColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
            UpdatePreview();
        }
    }

    private void UpdatePreview()
    {
        if (previewImage != null)
        {
            previewImage.color = _currentColor;
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