using UnityEngine;

public class BuildingSystemManager : MonoBehaviour
{
    public static BuildingSystemManager Instance { get; private set; }

    [Header("系统引用")]
    public MaterialInventory MaterialInventory;
    public BuildingLevelManager BuildingLevelManager;
    public BuildingExecutor BuildingExecutor;
    public BlockCustomizer BlockCustomizer;

    [Header("方块预制体")]
    public GameObject BlockPrefab;

    [Header("UI 引用")]
    public BuildingUI BuildingUI;

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
