using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance { get; private set; }

    [Header("蓝图选择")]
    public GameObject blueprintPanel;
    public Transform blueprintListContainer;
    public GameObject blueprintItemPrefab;

    [Header("材料管理")]
    public GameObject materialPanel;
    public Transform materialListContainer;
    public GameObject materialItemPrefab;

    [Header("建造控制")]
    public GameObject buildControlPanel;
    public Button buildButton;
    public Button clearButton;
    public Slider buildProgressSlider;
    public TextMeshProUGUI buildProgressText;
    public TextMeshProUGUI blueprintInfoText;
    public TextMeshProUGUI materialStatusText;
    public TextMeshProUGUI levelStatusText;

    [Header("等级显示")]
    public TextMeshProUGUI woodworkingLevelText;
    public TextMeshProUGUI constructionLevelText;
    public Button increaseWoodworkingButton;
    public Button increaseConstructionButton;

    [Header("错误提示")]
    public GameObject errorPanel;
    public TextMeshProUGUI errorText;
    public Button errorOkButton;

    private BlueprintData _selectedBlueprint;
    private List<BlueprintData> _availableBlueprints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        if (buildButton != null)
        {
            buildButton.onClick.AddListener(OnBuildButtonClicked);
        }

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(OnClearButtonClicked);
        }

        if (increaseWoodworkingButton != null)
        {
            increaseWoodworkingButton.onClick.AddListener(OnIncreaseWoodworkingClicked);
        }

        if (increaseConstructionButton != null)
        {
            increaseConstructionButton.onClick.AddListener(OnIncreaseConstructionClicked);
        }

        if (errorOkButton != null)
        {
            errorOkButton.onClick.AddListener(HideError);
        }

        if (errorPanel != null)
        {
            errorPanel.SetActive(false);
        }
    }

    private void SubscribeToEvents()
    {
        if (MaterialInventory.Instance != null)
        {
            MaterialInventory.Instance.OnMaterialChanged += OnMaterialChanged;
        }

        if (BuildingLevelManager.Instance != null)
        {
            BuildingLevelManager.Instance.OnWoodworkingLevelChanged += OnWoodworkingLevelChanged;
            BuildingLevelManager.Instance.OnConstructionLevelChanged += OnConstructionLevelChanged;
        }

        if (BuildingExecutor.Instance != null)
        {
            BuildingExecutor.Instance.OnBuildingStarted += OnBuildingStarted;
            BuildingExecutor.Instance.OnBuildingCompleted += OnBuildingCompleted;
            BuildingExecutor.Instance.OnBuildingProgress += OnBuildingProgress;
            BuildingExecutor.Instance.OnBuildingError += OnBuildingError;
        }

        if (BlockCustomizer.Instance != null && BlockCustomizer.Instance.colorPicker != null)
        {
            BlockCustomizer.Instance.colorPicker.OnColorApplied += OnColorApplied;
        }
    }

    private void LoadBlueprints()
    {
        _availableBlueprints = new List<BlueprintData>();
        BlueprintData[] blueprints = Resources.LoadAll<BlueprintData>("Blueprints");

        foreach (var blueprint in blueprints)
        {
            _availableBlueprints.Add(blueprint);
            CreateBlueprintItem(blueprint);
        }
    }

    private void CreateBlueprintItem(BlueprintData blueprint)
    {
        if (blueprintItemPrefab == null || blueprintListContainer == null) return;

        GameObject item = Instantiate(blueprintItemPrefab, blueprintListContainer);
        BlueprintItemUI itemUI = item.GetComponent<BlueprintItemUI>();

        if (itemUI != null)
        {
            itemUI.Initialize(blueprint, OnBlueprintSelected);
        }
    }

    private void OnBlueprintSelected(BlueprintData blueprint)
    {
        _selectedBlueprint = blueprint;
        UpdateBlueprintInfo();
        UpdateMaterialStatus();
        UpdateLevelStatus();
    }

    private void UpdateBlueprintInfo()
    {
        if (blueprintInfoText == null || _selectedBlueprint == null) return;

        string info = $"蓝图: {_selectedBlueprint.blueprintName}\n";
        info += $"尺寸: {_selectedBlueprint.width} x {_selectedBlueprint.height} x {_selectedBlueprint.depth}\n";
        info += $"方块总数: {_selectedBlueprint.GetTotalBlockCount()}\n";
        info += $"描述: {_selectedBlueprint.description}";

        blueprintInfoText.text = info;
    }

    private void UpdateMaterialStatus()
    {
        if (materialStatusText == null || _selectedBlueprint == null) return;

        bool sufficient = MaterialInventory.Instance.CheckMaterialSufficient(_selectedBlueprint.materialRequirements);

        string status = "材料需求:\n";

        foreach (var requirement in _selectedBlueprint.materialRequirements)
        {
            int currentCount = MaterialInventory.Instance.GetMaterialCount(requirement.Key);
            status += $"{requirement.Key}: {currentCount}/{requirement.Value}";
            
            if (currentCount < requirement.Value)
            {
                status += " [不足]";
            }
            status += "\n";
        }

        status += $"\n状态: {(sufficient ? "充足" : "不足")}";

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

        foreach (Transform child in materialListContainer)
        {
            Destroy(child.gameObject);
        }

        var materials = MaterialInventory.Instance.GetAllMaterials();

        foreach (var material in materials)
        {
            GameObject item = Instantiate(materialItemPrefab, materialListContainer);
            MaterialItemUI itemUI = item.GetComponent<MaterialItemUI>();

            if (itemUI != null)
            {
                itemUI.Initialize(material.Key, material.Value, OnAddMaterialClicked);
            }
        }
    }

    private void UpdateLevelDisplay()
    {
        if (woodworkingLevelText != null)
        {
            woodworkingLevelText.text = $"木工等级: {BuildingLevelManager.Instance.GetWoodworkingLevel()}";
        }

        if (constructionLevelText != null)
        {
            constructionLevelText.text = $"建筑等级: {BuildingLevelManager.Instance.GetConstructionLevel()}";
        }
    }

    private void OnMaterialChanged(MaterialType materialType, int count)
    {
        UpdateMaterialDisplay();
        UpdateMaterialStatus();
    }

    private void OnWoodworkingLevelChanged(int level)
    {
        UpdateLevelDisplay();
        UpdateLevelStatus();
    }

    private void OnConstructionLevelChanged(int level)
    {
        UpdateLevelDisplay();
        UpdateLevelStatus();
    }

    private void OnBuildButtonClicked()
    {
        if (_selectedBlueprint == null)
        {
            ShowError("请先选择一个蓝图");
            return;
        }

        if (BuildingExecutor.Instance == null)
        {
            ShowError("建造系统未初始化");
            return;
        }

        if (!BuildingExecutor.Instance.CheckCanBuild(_selectedBlueprint))
        {
            return;
        }

        bool materialsSufficient = BuildingExecutor.Instance.CheckMaterialsSufficient(_selectedBlueprint);

        if (materialsSufficient)
        {
            MaterialInventory.Instance.ConsumeMaterials(_selectedBlueprint.materialRequirements);
        }

        BuildingExecutor.Instance.StartBuilding(_selectedBlueprint, Vector3.zero);
    }

    private void OnClearButtonClicked()
    {
        if (BuildingExecutor.Instance != null)
        {
            BuildingExecutor.Instance.ClearBuilding();
        }
    }

    private void OnAddMaterialClicked(MaterialType materialType)
    {
        MaterialInventory.Instance.AddMaterial(materialType, 10);
    }

    private void OnIncreaseWoodworkingClicked()
    {
        BuildingLevelManager.Instance.AddWoodworkingLevel(1);
    }

    private void OnIncreaseConstructionClicked()
    {
        BuildingLevelManager.Instance.AddConstructionLevel(1);
    }

    private void OnBuildingStarted(BlueprintData blueprint)
    {
        if (buildButton != null)
        {
            buildButton.interactable = false;
        }
    }

    private void OnBuildingCompleted(BlueprintData blueprint)
    {
        if (buildButton != null)
        {
            buildButton.interactable = true;
        }

        if (buildProgressSlider != null)
        {
            buildProgressSlider.value = 1f;
        }

        if (buildProgressText != null)
        {
            buildProgressText.text = "建造完成!";
        }
    }

    private void OnBuildingProgress(float progress)
    {
        if (buildProgressSlider != null)
        {
            buildProgressSlider.value = progress;
        }

        if (buildProgressText != null)
        {
            buildProgressText.text = $"建造进度: {progress * 100:F1}%";
        }
    }

    private void OnBuildingError(string error)
    {
        ShowError(error);
    }

    private void OnColorApplied(Color color)
    {
        Debug.Log($"颜色已应用: {color}");
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
        }

        if (errorPanel != null)
        {
            errorPanel.SetActive(true);
        }
    }

    private void HideError()
    {
        if (errorPanel != null)
        {
            errorPanel.SetActive(false);
        }
    }
}