using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance { get; private set; }

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
        BuildButton?.onClick.AddListener(OnBuildButtonClicked);
        ClearButton?.onClick.AddListener(OnClearButtonClicked);
        IncreaseWoodworkingButton?.onClick.AddListener(OnIncreaseWoodworkingClicked);
        IncreaseConstructionButton?.onClick.AddListener(OnIncreaseConstructionClicked);
        ErrorOkButton?.onClick.AddListener(HideError);
        AddAllMaterialsButton?.onClick.AddListener(OnAddAllMaterialsClicked);

        if (ErrorPanel != null) ErrorPanel.SetActive(false);

        // 坐标输入框初始化
        if (MapXInput != null)
        {
            MapXInput.text = "0";
            MapXInput.onEndEdit.AddListener(_ => ParseMapPosition());
        }
        if (MapYInput != null)
        {
            MapYInput.text = "0";
            MapYInput.onEndEdit.AddListener(_ => ParseMapPosition());
        }
    }

    private void ParseMapPosition()
    {
        int x = 0, y = 0;
        if (MapXInput != null) int.TryParse(MapXInput.text, out x);
        if (MapYInput != null) int.TryParse(MapYInput.text, out y);
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

        if (BlockCustomizer.Instance?.ColorPicker != null)
            BlockCustomizer.Instance.ColorPicker.OnColorApplied += OnColorApplied;
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
        if (BlueprintInfoText == null || _selectedBlueprint == null) return;
        BlueprintInfoText.text =
            $"Blueprint: {_selectedBlueprint.BlueprintName}\n" +
            $"Size: {_selectedBlueprint.Width}x{_selectedBlueprint.Height}x{_selectedBlueprint.Depth}\n" +
            $"Total Blocks: {_selectedBlueprint.GetTotalBlockCount()}\n" +
            $"Description: {_selectedBlueprint.Description}";
    }

    private void UpdateMaterialStatus()
    {
        if (MaterialStatusText == null || _selectedBlueprint == null) return;

        string status = "Material Requirements:\n";
        foreach (var req in _selectedBlueprint.MaterialRequirements)
        {
            int cur = MaterialInventory.Instance.GetMaterialCount(req.Key);
            status += $"{req.Key}: {cur}/{req.Value}";
            if (cur < req.Value) status += " [Insufficient]";
            status += "\n";
        }
        MaterialStatusText.text = status;
    }

    private void UpdateLevelStatus()
    {
        if (LevelStatusText == null || _selectedBlueprint == null) return;
        LevelStatusText.text = BuildingLevelManager.Instance.GetLevelRequirementsText(_selectedBlueprint.RequiredLevel);
    }

    private void UpdateMaterialDisplay()
    {
        if (MaterialListContainer == null || MaterialItemPrefab == null) return;
        foreach (Transform child in MaterialListContainer) Destroy(child.gameObject);

        foreach (var mat in MaterialInventory.Instance.GetAllMaterials())
        {
            var item = Instantiate(MaterialItemPrefab, MaterialListContainer);
            item.GetComponent<MaterialItemUI>()?.Initialize(mat.Key, mat.Value, OnAddMaterialClicked);
        }
    }

    private void UpdateLevelDisplay()
    {
        if (WoodworkingLevelText != null)
            WoodworkingLevelText.text = $"Woodworking Level: {BuildingLevelManager.Instance.GetWoodworkingLevel()}";
        if (ConstructionLevelText != null)
            ConstructionLevelText.text = $"Construction Level: {BuildingLevelManager.Instance.GetConstructionLevel()}";
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
        if (BuildProgressSlider != null) BuildProgressSlider.value = progress;
        if (BuildProgressText != null)
        {
            if (progress <= 0f)
                BuildProgressText.text = "Not Started";
            else if (progress >= 1f)
                BuildProgressText.text = "Building Complete!";
            else
                BuildProgressText.text = $"Building Progress: {progress * 100:F1}%";
        }
    }

    // ---- 事件回调 ----

    private void OnMaterialChanged(MaterialType t, int count) { UpdateMaterialDisplay(); UpdateMaterialStatus(); }
    private void OnWoodworkingLevelChanged(int l) { UpdateLevelDisplay(); UpdateLevelStatus(); }
    private void OnConstructionLevelChanged(int l) { UpdateLevelDisplay(); UpdateLevelStatus(); }

    private void OnBuildingStarted(BlueprintData bp, Vector2Int pos)
    {
        if (pos == _currentMapPosition && BuildButton != null)
            BuildButton.interactable = false;
    }

    private void OnBuildingCompleted(BlueprintData bp, Vector2Int pos)
    {
        if (pos != _currentMapPosition) return;
        if (BuildButton != null) BuildButton.interactable = true;
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
        if (BuildButton != null) BuildButton.interactable = true;
        UpdateProgressDisplay();
    }

    private void OnBuildingResumed(BlueprintData bp, Vector2Int pos)
    {
        if (pos == _currentMapPosition && BuildButton != null)
            BuildButton.interactable = false;
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
    private void OnAddAllMaterialsClicked()
    {
        // 给所有材料类型添加200个
        foreach (MaterialType materialType in Enum.GetValues(typeof(MaterialType)))
        {
            MaterialInventory.Instance.AddMaterial(materialType, 200);
        }
    }
    private void OnIncreaseWoodworkingClicked() => BuildingLevelManager.Instance.AddWoodworkingLevel(1);
    private void OnIncreaseConstructionClicked() => BuildingLevelManager.Instance.AddConstructionLevel(1);

    private void ShowError(string msg)
    {
        if (ErrorText != null) ErrorText.text = msg;
        if (ErrorPanel != null) ErrorPanel.SetActive(true);
    }

    private void HideError()
    {
        if (ErrorPanel != null) ErrorPanel.SetActive(false);
    }
}
