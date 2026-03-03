using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BlueprintItemUI : MonoBehaviour
{
    [Header("UI元素")]
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI SizeText;
    public Button SelectButton;

    private BlueprintData _blueprint;
    private Action<BlueprintData> _onSelected;

    public void Initialize(BlueprintData blueprint, Action<BlueprintData> onSelected)
    {
        _blueprint = blueprint;
        _onSelected = onSelected;

        if (NameText != null)
        {
            NameText.text = blueprint.BlueprintName;
        }

        if (SizeText != null)
        {
            SizeText.text = $"{blueprint.Width}x{blueprint.Height}x{blueprint.Depth}";
        }

        if (SelectButton != null)
        {
            SelectButton.onClick.AddListener(onSelectClicked);
        }
    }

    private void onSelectClicked()
    {
        _onSelected?.Invoke(_blueprint);
    }
}