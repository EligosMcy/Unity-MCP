using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class BuildingUI : MonoBehaviour
{
    [Header("Blueprint Selection")]
    public GameObject BlueprintPanel;
    public Transform BlueprintListContainer;
    public GameObject BlueprintItemPrefab;

    [Header("Material Management")]
    public GameObject MaterialPanel;
    public Transform MaterialListContainer;
    public GameObject MaterialItemPrefab;
    public Button AddAllMaterialsButton;

    [Header("Build Control")]
    public GameObject BuildControlPanel;
    public Button BuildButton;
    public Button ClearButton;
    public Slider BuildProgressSlider;
    public TextMeshProUGUI BuildProgressText;
    public TextMeshProUGUI BlueprintInfoText;
    public TextMeshProUGUI MaterialStatusText;
    public TextMeshProUGUI LevelStatusText;

    [Header("Map Coordinate Input")]
    public TMP_InputField MapXInput;
    public TMP_InputField MapYInput;

    [Header("Level Display")]
    public TextMeshProUGUI WoodworkingLevelText;
    public TextMeshProUGUI ConstructionLevelText;
    public Button IncreaseWoodworkingButton;
    public Button IncreaseConstructionButton;

    [Header("Error Prompt")]
    public GameObject ErrorPanel;
    public TextMeshProUGUI ErrorText;
    public Button ErrorOkButton;

    [Header("Input Controls")]
    public InputActionProperty ModeSwitchInputActionProperty;

    [Header("Color Picker")]
    public ColorPickerUI ColorPickerUI;

    private BuildingSystemPresenter _presenter;
    private BuildingStateManager _stateManager;
    private BuildingSystemManager _systemManager;
    private List<BlueprintData> _availableBlueprints;

    private void Awake()
    {
        InitializeUI();
    }

    private void Start()
    {
        LoadBlueprints();
        SetupInput();
    }

    public void Initialize(BuildingSystemManager systemManager)
    {
        _systemManager = systemManager;
        _presenter = new BuildingSystemPresenter(systemManager, this);
        _presenter.Initialize();
    }

    public void SetPresenter(BuildingSystemPresenter presenter)
    {
        _presenter = presenter;
    }

    public void SetStateManager(BuildingStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    private void InitializeUI()
    {
        if (BuildButton != null)
            BuildButton.onClick.AddListener(OnBuildButtonClicked);

        if (ClearButton != null)
            ClearButton.onClick.AddListener(OnClearButtonClicked);

        if (AddAllMaterialsButton != null)
            AddAllMaterialsButton.onClick.AddListener(OnAddAllMaterialsClicked);

        if (IncreaseWoodworkingButton != null)
            IncreaseWoodworkingButton.onClick.AddListener(OnIncreaseWoodworkingClicked);

        if (IncreaseConstructionButton != null)
            IncreaseConstructionButton.onClick.AddListener(OnIncreaseConstructionClicked);

        if (ErrorOkButton != null)
            ErrorOkButton.onClick.AddListener(OnErrorOkClicked);

        if (MapXInput != null)
            MapXInput.onEndEdit.AddListener(OnMapXChanged);

        if (MapYInput != null)
            MapYInput.onEndEdit.AddListener(OnMapYChanged);

        if (ColorPickerUI != null)
            ColorPickerUI.OnColorApplied += OnColorApplied;

        if (ErrorPanel != null)
            ErrorPanel.SetActive(false);
    }

    private void SetupInput()
    {
        if (ModeSwitchInputActionProperty != null && ModeSwitchInputActionProperty.action != null)
        {
            ModeSwitchInputActionProperty.action.performed += OnModeSwitchPerformed;
            ModeSwitchInputActionProperty.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (ModeSwitchInputActionProperty != null && ModeSwitchInputActionProperty.action != null)
        {
            ModeSwitchInputActionProperty.action.performed -= OnModeSwitchPerformed;
            ModeSwitchInputActionProperty.action.Disable();
        }

        if (ColorPickerUI != null)
            ColorPickerUI.OnColorApplied -= OnColorApplied;

        _presenter?.Cleanup();
    }

    private void LoadBlueprints()
    {
        _availableBlueprints = new List<BlueprintData>();
        foreach (var bp in Resources.LoadAll<BlueprintData>("Blueprints"))
        {
            _availableBlueprints.Add(bp);
            CreateBlueprintItem(bp);
        }
    }

    private void CreateBlueprintItem(BlueprintData blueprint)
    {
        if (BlueprintItemPrefab == null || BlueprintListContainer == null) return;
        var item = Instantiate(BlueprintItemPrefab, BlueprintListContainer);
        item.GetComponent<BlueprintItemUI>()?.Initialize(blueprint, OnBlueprintItemClicked);
    }

    private void OnBlueprintItemClicked(BlueprintData blueprint)
    {
        _presenter?.OnBlueprintSelected(blueprint);
    }

    private void OnBuildButtonClicked()
    {
        _presenter?.OnBuildButtonClicked();
    }

    private void OnClearButtonClicked()
    {
        _presenter?.OnClearButtonClicked();
    }

    private void OnAddAllMaterialsClicked()
    {
        _presenter?.OnAddAllMaterialsClicked();
    }

    private void OnIncreaseWoodworkingClicked()
    {
        _presenter?.OnIncreaseWoodworkingClicked();
    }

    private void OnIncreaseConstructionClicked()
    {
        _presenter?.OnIncreaseConstructionClicked();
    }

    private void OnModeSwitchPerformed(InputAction.CallbackContext context)
    {
        _presenter?.OnModeToggleClicked();
    }

    private void OnMapXChanged(string value)
    {
        if (int.TryParse(value, out int x))
        {
            _presenter?.OnMapPositionChanged(x, _stateManager?.CurrentMapPosition.y ?? 0);
        }
    }

    private void OnMapYChanged(string value)
    {
        if (int.TryParse(value, out int y))
        {
            _presenter?.OnMapPositionChanged(_stateManager?.CurrentMapPosition.x ?? 0, y);
        }
    }

    private void OnColorApplied(Color color)
    {
        _presenter?.OnColorApplied(color);
    }

    private void OnErrorOkClicked()
    {
        if (ErrorPanel != null)
            ErrorPanel.SetActive(false);
    }

    private void OnAddMaterialClicked(MaterialType materialType)
    {
        _presenter?.OnAddMaterialClicked(materialType);
    }

    public void UpdateMaterialDisplay()
    {
        if (MaterialListContainer == null || MaterialItemPrefab == null) return;
        foreach (Transform child in MaterialListContainer) Destroy(child.gameObject);

        if (_systemManager?.MaterialInventory == null) return;

        foreach (var mat in _systemManager.MaterialInventory.GetAllMaterials())
        {
            var item = Instantiate(MaterialItemPrefab, MaterialListContainer);
            item.GetComponent<MaterialItemUI>()?.Initialize(mat.Key, mat.Value, OnAddMaterialClicked);
        }
    }

    public void UpdateBlueprintInfo(BlueprintData blueprint)
    {
        if (BlueprintInfoText == null || blueprint == null) return;
        BlueprintInfoText.text =
            $"Blueprint: {blueprint.BlueprintName}\n" +
            $"Size: {blueprint.Width}x{blueprint.Height}x{blueprint.Depth}\n" +
            $"Total Blocks: {blueprint.GetTotalBlockCount()}\n" +
            $"Description: {blueprint.Description}";
    }

    public void UpdateMaterialStatus()
    {
        if (MaterialStatusText == null || _stateManager?.SelectedBlueprint == null) return;
        var requirements = _stateManager.SelectedBlueprint.MaterialRequirements;
        string text = "Material Status:\n";
        foreach (var req in requirements)
        {
            int current = _systemManager?.MaterialInventory?.GetMaterialCount(req.Key) ?? 0;
            text += $"{req.Key}: {current}/{req.Value}";
            if (current < req.Value) text += " [Insufficient]";
            text += "\n";
        }
        MaterialStatusText.text = text.Trim();
    }

    public void UpdateLevelDisplay()
    {
        if (WoodworkingLevelText == null || ConstructionLevelText == null) return;
        WoodworkingLevelText.text = $"Woodworking: {_systemManager?.BuildingLevelManager?.GetWoodworkingLevel() ?? 1}";
        ConstructionLevelText.text = $"Construction: {_systemManager?.BuildingLevelManager?.GetConstructionLevel() ?? 1}";
    }

    public void UpdateLevelStatus()
    {
        if (LevelStatusText == null || _stateManager?.SelectedBlueprint == null) return;
        LevelStatusText.text = _systemManager?.BuildingLevelManager?.GetLevelRequirementsText(_stateManager.SelectedBlueprint.RequiredLevel) ?? "";
    }

    public void UpdateProgressDisplay()
    {
        if (_systemManager?.BuildingExecutor == null) return;
        float progress = _systemManager.BuildingExecutor.GetSessionProgress(_stateManager?.CurrentMapPosition ?? Vector2Int.zero);
        SetProgressUI(progress);
    }

    public void SetProgressUI(float progress)
    {
        if (BuildProgressSlider != null)
            BuildProgressSlider.value = progress;
        if (BuildProgressText != null)
            BuildProgressText.text = $"{(progress * 100):F0}%";
    }

    public void SetBuildButtonInteractable(bool interactable)
    {
        if (BuildButton != null)
            BuildButton.interactable = interactable;
    }

    public void UpdateModeDisplay(BuildingMode mode)
    {
        switch (mode)
        {
            case BuildingMode.Color:
                if (ColorPickerUI != null) ColorPickerUI.Show();
                if (BlueprintPanel != null) BlueprintPanel.SetActive(false);
                if (MaterialPanel != null) MaterialPanel.SetActive(false);
                if (BuildControlPanel != null) BuildControlPanel.SetActive(false);
                break;

            case BuildingMode.Build:
                if (ColorPickerUI != null) ColorPickerUI.Hide();
                if (BlueprintPanel != null) BlueprintPanel.SetActive(true);
                if (MaterialPanel != null) MaterialPanel.SetActive(true);
                if (BuildControlPanel != null) BuildControlPanel.SetActive(true);
                break;
        }
    }

    public void ShowError(string msg)
    {
        if (ErrorText != null) ErrorText.text = msg;
        if (ErrorPanel != null) ErrorPanel.SetActive(true);
    }
}
