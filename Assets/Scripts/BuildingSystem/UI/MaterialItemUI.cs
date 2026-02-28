using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MaterialItemUI : MonoBehaviour
{
    [Header("UI元素")]
    public TextMeshProUGUI MaterialNameText;
    public TextMeshProUGUI CountText;
    public Button AddButton;

    private MaterialType _materialType;
    private Action<MaterialType> _onAddClicked;

    public void Initialize(MaterialType materialType, int count, Action<MaterialType> onAddClicked)
    {
        _materialType = materialType;
        _onAddClicked = onAddClicked;

        if (MaterialNameText != null)
        {
            MaterialNameText.text = materialType.ToString();
        }

        if (CountText != null)
        {
            CountText.text = count.ToString();
        }

        if (AddButton != null)
        {
            AddButton.onClick.AddListener(OnAddClicked);
        }
    }

    private void OnAddClicked()
    {
        _onAddClicked?.Invoke(_materialType);
    }

    public void UpdateCount(int count)
    {
        if (CountText != null)
        {
            CountText.text = count.ToString();
        }
    }
}