using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Color Picker")]
    public ColorPickerUI ColorPickerUI;

    private BuildingSystemPresenter _presenter;
    private BuildingStateManager _stateManager;
    private BuildingSystemManager _systemManager;
    private BuildingInputProvider _inputProvider;
    private List<BlueprintData> _availableBlueprints;
    private Dictionary<MaterialType, MaterialItemUI> _materialItemCache = new Dictionary<MaterialType, MaterialItemUI>();

    private void Awake()
    {
        initializeUI();
    }

    private void Start()
    {
        loadBlueprints();
        setupInput();
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

    private void initializeUI()
    {
        if (BuildButton != null)
            BuildButton.onClick.AddListener(onBuildButtonClicked);

        if (ClearButton != null)
            ClearButton.onClick.AddListener(onClearButtonClicked);

        if (AddAllMaterialsButton != null)
            AddAllMaterialsButton.onClick.AddListener(onAddAllMaterialsClicked);

        if (IncreaseWoodworkingButton != null)
            IncreaseWoodworkingButton.onClick.AddListener(onIncreaseWoodworkingClicked);

        if (IncreaseConstructionButton != null)
            IncreaseConstructionButton.onClick.AddListener(onIncreaseConstructionClicked);

        if (ErrorOkButton != null)
            ErrorOkButton.onClick.AddListener(onErrorOkClicked);

        if (MapXInput != null)
            MapXInput.onEndEdit.AddListener(onMapXChanged);

        if (MapYInput != null)
            MapYInput.onEndEdit.AddListener(onMapYChanged);

        if (ColorPickerUI != null)
            ColorPickerUI.OnColorApplied += onColorApplied;

        if (ErrorPanel != null)
            ErrorPanel.SetActive(false);
    }

    private void setupInput()
    {
        if (_systemManager != null)
        {
            _inputProvider = _systemManager.GetComponent<BuildingInputProvider>();
            if (_inputProvider != null)
            {
                _inputProvider.OnModeSwitched += onModeSwitchPerformed;
            }
        }
    }

    private void OnDestroy()
    {
        if (_inputProvider != null)
        {
            _inputProvider.OnModeSwitched -= onModeSwitchPerformed;
        }

        if (ColorPickerUI != null)
            ColorPickerUI.OnColorApplied -= onColorApplied;

        _presenter?.Cleanup();
    }

    private void loadBlueprints()
    {
        _availableBlueprints = new List<BlueprintData>();
        foreach (var bp in Resources.LoadAll<BlueprintData>("Blueprints"))
        {
            _availableBlueprints.Add(bp);
            createBlueprintItem(bp);
        }
    }

    private void createBlueprintItem(BlueprintData blueprint)
    {
        if (BlueprintItemPrefab == null || BlueprintListContainer == null) return;
        var item = Instantiate(BlueprintItemPrefab, BlueprintListContainer);
        item.GetComponent<BlueprintItemUI>()?.Initialize(blueprint, onBlueprintItemClicked);
    }

    private void onBlueprintItemClicked(BlueprintData blueprint)
    {
        _presenter?.OnBlueprintSelected(blueprint);
    }

    private void onBuildButtonClicked()
    {
        _presenter?.OnBuildButtonClicked();
    }

    private void onClearButtonClicked()
    {
        _presenter?.OnClearButtonClicked();
    }

    private void onAddAllMaterialsClicked()
    {
        _presenter?.OnAddAllMaterialsClicked();
    }

    private void onIncreaseWoodworkingClicked()
    {
        _presenter?.OnIncreaseWoodworkingClicked();
    }

    private void onIncreaseConstructionClicked()
    {
        _presenter?.OnIncreaseConstructionClicked();
    }

    private void onModeSwitchPerformed()
    {
        _presenter?.OnModeToggleClicked();
    }

    private void onMapXChanged(string value)
    {
        if (int.TryParse(value, out int x))
        {
            _presenter?.OnMapPositionChanged(x, _stateManager?.CurrentMapPosition.y ?? 0);
        }
    }

    private void onMapYChanged(string value)
    {
        if (int.TryParse(value, out int y))
        {
            _presenter?.OnMapPositionChanged(_stateManager?.CurrentMapPosition.x ?? 0, y);
        }
    }

    private void onColorApplied(Color color)
    {
        _presenter?.OnColorApplied(color);
    }

    private void onErrorOkClicked()
    {
        if (ErrorPanel != null)
            ErrorPanel.SetActive(false);
    }

    private void onAddMaterialClicked(MaterialType materialType)
    {
        _presenter?.OnAddMaterialClicked(materialType);
    }

    public void UpdateMaterialDisplay()
    {
        if (MaterialListContainer == null || MaterialItemPrefab == null) return;

        if (_systemManager.MaterialInventory == null) return;

        var materials = _systemManager.MaterialInventory.GetAllMaterials();
        var processedTypes = new HashSet<MaterialType>();

        foreach (var mat in materials)
        {
            processedTypes.Add(mat.Key);

            if (_materialItemCache.TryGetValue(mat.Key, out var existingItem))
            {
                existingItem.UpdateCount(mat.Value);
            }
            else
            {
                var item = Instantiate(MaterialItemPrefab, MaterialListContainer);
                var ui = item.GetComponent<MaterialItemUI>();
                ui.Initialize(mat.Key, mat.Value, onAddMaterialClicked);
                _materialItemCache[mat.Key] = ui;
            }
        }

        var toRemove = new List<MaterialType>();
        foreach (var key in _materialItemCache.Keys)
        {
            if (!processedTypes.Contains(key))
            {
                toRemove.Add(key);
            }
        }

        foreach (var type in toRemove)
        {
            if (_materialItemCache.TryGetValue(type, out var item))
            {
                Destroy(item.gameObject);
                _materialItemCache.Remove(type);
            }
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
