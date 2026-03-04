using UnityEngine;
using UnityEngine.Serialization;

public class BuildingSystemManager : MonoBehaviour
{
    public static BuildingSystemManager Instance { get; private set; }

    [Header("系统引用")]
    [FormerlySerializedAs("MaterialInventory")]
    [SerializeField] private MaterialInventory _materialInventory;
    public MaterialInventory MaterialInventory 
    { 
        get => _materialInventory; 
        private set => _materialInventory = value; 
    }

    [FormerlySerializedAs("BuildingLevelManager")]
    [SerializeField] private BuildingLevelManager _buildingLevelManager;
    public BuildingLevelManager BuildingLevelManager 
    { 
        get => _buildingLevelManager; 
        private set => _buildingLevelManager = value; 
    }

    [FormerlySerializedAs("BuildingExecutor")]
    [SerializeField] private BuildingExecutor _buildingExecutor;
    public BuildingExecutor BuildingExecutor 
    { 
        get => _buildingExecutor; 
        private set => _buildingExecutor = value; 
    }

    [FormerlySerializedAs("BlockCustomizer")]
    [SerializeField] private BlockCustomizer _blockCustomizer;
    public BlockCustomizer BlockCustomizer 
    { 
        get => _blockCustomizer; 
        private set => _blockCustomizer = value; 
    }

    [Header("方块预制体")]
    [FormerlySerializedAs("BlockPrefab")]
    [SerializeField] private GameObject _blockPrefab;
    public GameObject BlockPrefab 
    { 
        get => _blockPrefab; 
        private set => _blockPrefab = value; 
    }

    [Header("UI 引用")]
    [FormerlySerializedAs("BuildingUI")]
    [SerializeField] private BuildingUI _buildingUI;
    public BuildingUI BuildingUI 
    { 
        get => _buildingUI; 
        private set => _buildingUI = value; 
    }

    public IMaterialInventory MaterialInventoryInterface => MaterialInventory;
    public IBuildingLevelManager LevelManagerInterface => BuildingLevelManager;
    public IBuildingExecutor ExecutorInterface => BuildingExecutor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            initializeSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void initializeSystem()
    {
        if (MaterialInventory == null)
        {
            MaterialInventory = gameObject.AddComponent<MaterialInventory>();
        }

        MaterialInventory.InitializeMaterials();

        if (BuildingLevelManager == null)
        {
            BuildingLevelManager = gameObject.AddComponent<BuildingLevelManager>();
        }

        if (BuildingExecutor == null)
        {
            BuildingExecutor = gameObject.AddComponent<BuildingExecutor>();
            BuildingExecutor.BlockPrefab = BlockPrefab;
        }

        if (BlockCustomizer == null)
        {
            BlockCustomizer = gameObject.AddComponent<BlockCustomizer>();
        }

        BuildingExecutor.Initialize(MaterialInventory, BuildingLevelManager);

        if (BuildingUI != null)
        {
            BuildingUI.Initialize(this);
        }
    }
}
