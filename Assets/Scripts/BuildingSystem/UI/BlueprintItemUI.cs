using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BlueprintItemUI : MonoBehaviour
{
    [Header("UI元素")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI sizeText;
    public Button selectButton;

    private BlueprintData _blueprint;
    private Action<BlueprintData> _onSelected;

    public void Initialize(BlueprintData blueprint, Action<BlueprintData> onSelected)
    {
        _blueprint = blueprint;
        _onSelected = onSelected;

        if (nameText != null)
        {
            nameText.text = blueprint.blueprintName;
        }

        if (sizeText != null)
        {
            sizeText.text = $"{blueprint.width}x{blueprint.height}x{blueprint.depth}";
        }

        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectClicked);
        }
    }

    private void OnSelectClicked()
    {
        _onSelected?.Invoke(_blueprint);
    }
}