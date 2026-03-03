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
    public Slider MapXSlider;
    public Slider MapYSlider;
    public TextMeshProUGUI MapXValueText;
    public TextMeshProUGUI MapYValueText;

    [Header("Position Preview")]
    public GameObject GizmoCubePrefab;
    private GameObject _positionPreviewInstance;
    private Renderer _positionPreviewRenderer;

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
        createPositionPreview();
    }

    private void Start()
    {
        loadBlueprints();
        setupInput();
        initializeMapSliders();
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
        if (_stateManager != null)
        {
            _stateManager.OnBlueprintSelected += onBlueprintSelected;
            _stateManager.OnMapPositionChanged += onMapPositionChangedForPreview;
        }
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

        if (MapXSlider != null)
         {
             MapXSlider.minValue = -10;
             MapXSlider.maxValue = 10;
             MapXSlider.wholeNumbers = true;
             MapXSlider.onValueChanged.AddListener(onMapXChanged);
         }

         if (MapYSlider != null)
         {
             MapYSlider.minValue = -10;
             MapYSlider.maxValue = 10;
             MapYSlider.wholeNumbers = true;
             MapYSlider.onValueChanged.AddListener(onMapYChanged);
         }

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

        if (_stateManager != null)
        {
            _stateManager.OnBlueprintSelected -= onBlueprintSelected;
            _stateManager.OnMapPositionChanged -= onMapPositionChangedForPreview;
        }

        _presenter?.Cleanup();
    }

    private void onBlueprintSelected(BlueprintData blueprint)
    {
        updatePositionPreviewSize(blueprint);
    }

    private void onMapPositionChangedForPreview(Vector2Int position)
    {
        updatePositionPreview(position.x, position.y);
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

    private void onMapXChanged(float value)
    {
        int x = Mathf.RoundToInt(value);
        int y = _stateManager?.CurrentMapPosition.y ?? 0;
        updateMapXValueText(x);
        updatePositionPreview(x, y);
        _presenter?.OnMapPositionChanged(x, y);
    }

    private void onMapYChanged(float value)
    {
        int x = _stateManager?.CurrentMapPosition.x ?? 0;
        int y = Mathf.RoundToInt(value);
        updateMapYValueText(y);
        updatePositionPreview(x, y);
        _presenter?.OnMapPositionChanged(x, y);
    }

    private void initializeMapSliders()
    {
        int startX = _stateManager?.CurrentMapPosition.x ?? 0;
        int startY = _stateManager?.CurrentMapPosition.y ?? 0;

        if (MapXSlider != null)
        {
            MapXSlider.value = startX;
            updateMapXValueText(startX);
        }

        if (MapYSlider != null)
        {
            MapYSlider.value = startY;
            updateMapYValueText(startY);
        }

        updatePositionPreview(startX, startY);
        updatePositionPreviewSize(_stateManager?.SelectedBlueprint);
    }

    private void updateMapXValueText(int value)
    {
        if (MapXValueText != null)
            MapXValueText.text = value.ToString();
    }

    private void updateMapYValueText(int value)
    {
        if (MapYValueText != null)
            MapYValueText.text = value.ToString();
    }

    private void updatePositionPreview(int x, int y)
    {
        if (_positionPreviewInstance != null)
        {
            float height = getCurrentBlueprintHeight();
            _positionPreviewInstance.transform.position = new Vector3(x, height / 2.0f, y);
        }

        if (_positionPreviewRenderer != null)
        {
            _positionPreviewRenderer.enabled = true;
        }
    }

    private float getCurrentBlueprintHeight()
    {
        if (_stateManager?.SelectedBlueprint != null)
        {
            return _stateManager.SelectedBlueprint.Height;
        }
        return 1f;
    }

    private void createPositionPreview()
    {
        if (GizmoCubePrefab != null)
        {
            _positionPreviewInstance = Instantiate(GizmoCubePrefab);
            _positionPreviewRenderer = _positionPreviewInstance.GetComponent<Renderer>();
            _positionPreviewRenderer.enabled = false;
        }
    }

    private void updatePositionPreviewSize(BlueprintData blueprint)
    {
        if (_positionPreviewInstance == null) return;

        int width = 1;
        int height = 1;
        int depth = 1;

        if (blueprint != null)
        {
            width = blueprint.Width;
            height = blueprint.Height;
            depth = blueprint.Depth;
        }

        _positionPreviewInstance.transform.localScale = new Vector3(width, height, depth);

        int currentX = _stateManager?.CurrentMapPosition.x ?? 0;
        int currentY = _stateManager?.CurrentMapPosition.y ?? 0;
        _positionPreviewInstance.transform.position = new Vector3(currentX, height / 2.0f, currentY);
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
                if (_positionPreviewRenderer != null) _positionPreviewRenderer.enabled = false;
                break;

            case BuildingMode.Build:
                if (ColorPickerUI != null) ColorPickerUI.Hide();
                if (BlueprintPanel != null) BlueprintPanel.SetActive(true);
                if (MaterialPanel != null) MaterialPanel.SetActive(true);
                if (BuildControlPanel != null) BuildControlPanel.SetActive(true);
                if (_positionPreviewRenderer != null) _positionPreviewRenderer.enabled = true;
                break;
        }
    }

    public void ShowError(string msg)
    {
        if (ErrorText != null) ErrorText.text = msg;
        if (ErrorPanel != null) ErrorPanel.SetActive(true);
    }
}
