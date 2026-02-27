using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance { get; private set; }

    [Header("Blueprint Selection")]
    public GameObject blueprintPanel;
    public Transform blueprintListContainer;
    public GameObject blueprintItemPrefab;

    [Header("Material Management")]
    public GameObject materialPanel;
    public Transform materialListContainer;
    public GameObject materialItemPrefab;

    [Header("Build Control")]
    public GameObject buildControlPanel;
    public Button buildButton;
    public Button clearButton;
    public Slider buildProgressSlider;
    public TextMeshProUGUI buildProgressText;
    public TextMeshProUGUI blueprintInfoText;
    public TextMeshProUGUI materialStatusText;
    public TextMeshProUGUI levelStatusText;

    [Header("Map Coordinate Input")]
    public TMP_InputField mapXInput;
    public TMP_InputField mapYInput;

    [Header("Level Display")]
    public TextMeshProUGUI woodworkingLevelText;
    public TextMeshProUGUI constructionLevelText;
    public Button increaseWoodworkingButton;
    public Button increaseConstructionButton;

    [Header("Error Prompt")]
    public GameObject errorPanel;
    public TextMeshProUGUI errorText;
    public Button errorOkButton;

    private BlueprintData _selectedBlueprint;
    private List<BlueprintData> _availableBlueprints;

    // 当前选定的地图坐标，默认 (0,0)
    private Vector2Int _currentMapPosition = Vector2Int.zero;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeUI();
        SubscribeToEvents();
        LoadBlueprints();
        UpdateMaterialDisplay();
        UpdateLevelDisplay();
    }

    private void InitializeUI()
    {
        buildButton?.onClick.AddListener(OnBuildButtonClicked);
        clearButton?.onClick.AddListener(OnClearButtonClicked);
        increaseWoodworkingButton?.onClick.AddListener(OnIncreaseWoodworkingClicked);
        increaseConstructionButton?.onClick.AddListener(OnIncreaseConstructionClicked);
        errorOkButton?.onClick.AddListener(HideError);

        if (errorPanel != null) errorPanel.SetActive(false);

        // 坐标输入框初始化
        if (mapXInput != null)
        {
            mapXInput.text = "0";
            mapXInput.onEndEdit.AddListener(_ => ParseMapPosition());
        }
        if (mapYInput != null)
        {
            mapYInput.text = "0";
            mapYInput.onEndEdit.AddListener(_ => ParseMapPosition());
        }
    }

    private void ParseMapPosition()
    {
        int x = 0, y = 0;
        if (mapXInput != null) int.TryParse(mapXInput.text, out x);
        if (mapYInput != null) int.TryParse(mapYInput.text, out y);
        _currentMapPosition = new Vector2Int(x, y);
        UpdateProgressDisplay();
    }

    private void SubscribeToEvents()
    {
        if (MaterialInventory.Instance != null)
            MaterialInventory.Instance.OnMaterialChanged += OnMaterialChanged;

        if (BuildingLevelManager.Instance != null)
        {
            BuildingLevelManager.Instance.OnWoodworkingLevelChanged += OnWoodworkingLevelChanged;
            BuildingLevelManager.Instance.OnConstructionLevelChanged += OnConstructionLevelChanged;
        }

        if (BuildingExecutor.Instance != null)
        {
            BuildingExecutor.Instance.OnBuildingStarted  += OnBuildingStarted;
            BuildingExecutor.Instance.OnBuildingCompleted += OnBuildingCompleted;
            BuildingExecutor.Instance.OnBuildingProgress  += OnBuildingProgress;
            BuildingExecutor.Instance.OnBuildingError     += OnBuildingError;
            BuildingExecutor.Instance.OnBuildingPaused    += OnBuildingPaused;
            BuildingExecutor.Instance.OnBuildingResumed   += OnBuildingResumed;
        }

        if (BlockCustomizer.Instance?.colorPicker != null)
            BlockCustomizer.Instance.colorPicker.OnColorApplied += OnColorApplied;
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
        if (blueprintItemPrefab == null || blueprintListContainer == null) return;
        var item = Instantiate(blueprintItemPrefab, blueprintListContainer);
        item.GetComponent<BlueprintItemUI>()?.Initialize(blueprint, OnBlueprintSelected);
    }

    private void OnBlueprintSelected(BlueprintData blueprint)
    {
        _selectedBlueprint = blueprint;
        UpdateBlueprintInfo();
        UpdateMaterialStatus();
        UpdateLevelStatus();
        UpdateProgressDisplay();
    }

    // ---- 显示更新 ----

    private void UpdateBlueprintInfo()
    {
        if (blueprintInfoText == null || _selectedBlueprint == null) return;
        blueprintInfoText.text =
            $"Blueprint: {_selectedBlueprint.blueprintName}\n" +
            $"Size: {_selectedBlueprint.width}x{_selectedBlueprint.height}x{_selectedBlueprint.depth}\n" +
            $"Total Blocks: {_selectedBlueprint.GetTotalBlockCount()}\n" +
            $"Description: {_selectedBlueprint.description}";
    }

    private void UpdateMaterialStatus()
    {
        if (materialStatusText == null || _selectedBlueprint == null) return;

        string status = "Material Requirements:\n";
        foreach (var req in _selectedBlueprint.materialRequirements)
        {
            int cur = MaterialInventory.Instance.GetMaterialCount(req.Key);
            status += $"{req.Key}: {cur}/{req.Value}";
            if (cur < req.Value) status += " [Insufficient]";
            status += "\n";
        }
        materialStatusText.text = status;
    }

    private void UpdateLevelStatus()
    {
        if (levelStatusText == null || _selectedBlueprint == null) return;
        levelStatusText.text = BuildingLevelManager.Instance.GetLevelRequirementsText(_selectedBlueprint.requiredLevel);
    }

    private void UpdateMaterialDisplay()
    {
        if (materialListContainer == null || materialItemPrefab == null) return;
        foreach (Transform child in materialListContainer) Destroy(child.gameObject);

        foreach (var mat in MaterialInventory.Instance.GetAllMaterials())
        {
            var item = Instantiate(materialItemPrefab, materialListContainer);
            item.GetComponent<MaterialItemUI>()?.Initialize(mat.Key, mat.Value, OnAddMaterialClicked);
        }
    }

    private void UpdateLevelDisplay()
    {
        if (woodworkingLevelText != null)
            woodworkingLevelText.text = $"Woodworking Level: {BuildingLevelManager.Instance.GetWoodworkingLevel()}";
        if (constructionLevelText != null)
            constructionLevelText.text = $"Construction Level: {BuildingLevelManager.Instance.GetConstructionLevel()}";
    }

    /// 显示当前坐标建造进度（包括未完成的暂停任务）
    private void UpdateProgressDisplay()
    {
        if (BuildingExecutor.Instance == null) return;
        float progress = BuildingExecutor.Instance.GetSessionProgress(_currentMapPosition);
        SetProgressUI(progress);
    }

    private void SetProgressUI(float progress)
    {
        if (buildProgressSlider != null) buildProgressSlider.value = progress;
        if (buildProgressText != null)
        {
            if (progress <= 0f)
                buildProgressText.text = "Not Started";
            else if (progress >= 1f)
                buildProgressText.text = "Building Complete!";
            else
                buildProgressText.text = $"Building Progress: {progress * 100:F1}%";
        }
    }

    // ---- 事件回调 ----

    private void OnMaterialChanged(MaterialType t, int count) { UpdateMaterialDisplay(); UpdateMaterialStatus(); }
    private void OnWoodworkingLevelChanged(int l) { UpdateLevelDisplay(); UpdateLevelStatus(); }
    private void OnConstructionLevelChanged(int l) { UpdateLevelDisplay(); UpdateLevelStatus(); }

    private void OnBuildingStarted(BlueprintData bp, Vector2Int pos)
    {
        if (pos == _currentMapPosition && buildButton != null)
            buildButton.interactable = false;
    }

    private void OnBuildingCompleted(BlueprintData bp, Vector2Int pos)
    {
        if (pos != _currentMapPosition) return;
        if (buildButton != null) buildButton.interactable = true;
        SetProgressUI(1f);
    }

    private void OnBuildingProgress(float progress, Vector2Int pos)
    {
        if (pos == _currentMapPosition) SetProgressUI(progress);
    }

    private void OnBuildingPaused(BlueprintData bp, Vector2Int pos)
    {
        // 暂停时恢复按钮可点击，展示当前进度
        if (pos != _currentMapPosition) return;
        if (buildButton != null) buildButton.interactable = true;
        UpdateProgressDisplay();
    }

    private void OnBuildingResumed(BlueprintData bp, Vector2Int pos)
    {
        if (pos == _currentMapPosition && buildButton != null)
            buildButton.interactable = false;
    }

    private void OnBuildingError(string error) => ShowError(error);
    private void OnColorApplied(Color color) => Debug.Log($"Color applied: {color}");

    // ---- 按钮事件 ----

    private void OnBuildButtonClicked()
    {
        if (_selectedBlueprint == null) { ShowError("Please select a blueprint first"); return; }
        if (BuildingExecutor.Instance == null) { ShowError("Building system not initialized"); return; }
        if (!BuildingExecutor.Instance.CheckCanBuild(_selectedBlueprint)) return;

        // 直接启动——材料消耗在协程里逐块进行
        BuildingExecutor.Instance.StartBuilding(_selectedBlueprint, _currentMapPosition);
    }

    private void OnClearButtonClicked()
    {
        BuildingExecutor.Instance?.ClearBuilding(_currentMapPosition);
        SetProgressUI(0f);
    }

    private void OnAddMaterialClicked(MaterialType t) => MaterialInventory.Instance.AddMaterial(t, 10);
    private void OnIncreaseWoodworkingClicked() => BuildingLevelManager.Instance.AddWoodworkingLevel(1);
    private void OnIncreaseConstructionClicked() => BuildingLevelManager.Instance.AddConstructionLevel(1);

    private void ShowError(string msg)
    {
        if (errorText != null) errorText.text = msg;
        if (errorPanel != null) errorPanel.SetActive(true);
    }

    private void HideError()
    {
        if (errorPanel != null) errorPanel.SetActive(false);
    }
}
