using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MaterialItemUI : MonoBehaviour
{
    [Header("UI元素")]
    public TextMeshProUGUI materialNameText;
    public TextMeshProUGUI countText;
    public Button addButton;

    private MaterialType _materialType;
    private Action<MaterialType> _onAddClicked;

    public void Initialize(MaterialType materialType, int count, Action<MaterialType> onAddClicked)
    {
        _materialType = materialType;
        _onAddClicked = onAddClicked;

        if (materialNameText != null)
        {
            materialNameText.text = materialType.ToString();
        }

        if (countText != null)
        {
            countText.text = count.ToString();
        }

        if (addButton != null)
        {
            addButton.onClick.AddListener(OnAddClicked);
        }
    }

    private void OnAddClicked()
    {
        _onAddClicked?.Invoke(_materialType);
    }

    public void UpdateCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }
}