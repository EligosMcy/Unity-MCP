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
    private bool _isUpdatingSliders;

    public event UnityAction<Color> OnColorApplied;

    private void Awake()
    {
        // 自动查找UI元素
        if (Panel == null)
        {
            Panel = transform.parent.gameObject;
        }

        if (PreviewImage == null)
        {
            PreviewImage = transform.parent.Find("PreviewImage").GetComponent<Image>();
        }

        if (RedSlider == null)
        {
            RedSlider = transform.parent.Find("RedSlider").GetComponent<Slider>();
        }

        if (GreenSlider == null)
        {
            GreenSlider = transform.parent.Find("GreenSlider").GetComponent<Slider>();
        }

        if (BlueSlider == null)
        {
            BlueSlider = transform.parent.Find("BlueSlider").GetComponent<Slider>();
        }

        if (ApplyButton == null)
        {
            ApplyButton = transform.parent.Find("ApplyButton").GetComponent<Button>();
        }

        if (CancelButton == null)
        {
            CancelButton = transform.parent.Find("CancelButton").GetComponent<Button>();
        }

        if (Panel != null)
        {
            Panel.SetActive(false);
        }

        if (RedSlider != null)
        {
            RedSlider.onValueChanged.AddListener(updateColor);
        }

        if (GreenSlider != null)
        {
            GreenSlider.onValueChanged.AddListener(updateColor);
        }

        if (BlueSlider != null)
        {
            BlueSlider.onValueChanged.AddListener(updateColor);
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

    public void Show()
    {
        if (Panel != null)
        {
            Panel.SetActive(true);
        }
    }

    public void SetInitialColor(Color initialColor)
    {
        _currentColor = initialColor;
        updateSliders();
        updatePreview();
    }

    public void Hide()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);
        }
    }

    private void updateSliders()
    {
        _isUpdatingSliders = true;

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

        _isUpdatingSliders = false;
    }

    private void updateColor(float value)
    {
        if (_isUpdatingSliders)
        {
            return;
        }

        if (RedSlider != null && GreenSlider != null && BlueSlider != null)
        {
            _currentColor = new Color(RedSlider.value, GreenSlider.value, BlueSlider.value);
            updatePreview();
        }
    }

    private void updatePreview()
    {
        if (PreviewImage != null)
        {
            PreviewImage.color = _currentColor;
        }
    }

    private void Apply()
    {
        OnColorApplied?.Invoke(_currentColor);
    }

    private void Cancel()
    {
    }
}