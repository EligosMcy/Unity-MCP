using UnityEngine;

public class BuildingSystemManager : MonoBehaviour
{
    public static BuildingSystemManager Instance { get; private set; }

    [Header("系统引用")]
    public MaterialInventory materialInventory;
    public BuildingLevelManager buildingLevelManager;
    public BuildingExecutor buildingExecutor;
    public BlockCustomizer blockCustomizer;
    public BuildingUI buildingUI;

    [Header("方块预制体")]
    public GameObject blockPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSystems()
    {
        if (materialInventory == null)
        {
            materialInventory = FindObjectOfType<MaterialInventory>();
            if (materialInventory == null)
            {
                GameObject go = new GameObject("MaterialInventory");
                materialInventory = go.AddComponent<MaterialInventory>();
            }
        }

        if (buildingLevelManager == null)
        {
            buildingLevelManager = FindObjectOfType<BuildingLevelManager>();
            if (buildingLevelManager == null)
            {
                GameObject go = new GameObject("BuildingLevelManager");
                buildingLevelManager = go.AddComponent<BuildingLevelManager>();
            }
        }

        if (buildingExecutor == null)
        {
            buildingExecutor = FindObjectOfType<BuildingExecutor>();
            if (buildingExecutor == null)
            {
                GameObject go = new GameObject("BuildingExecutor");
                buildingExecutor = go.AddComponent<BuildingExecutor>();
            }
        }

        if (blockCustomizer == null)
        {
            blockCustomizer = FindObjectOfType<BlockCustomizer>();
            if (blockCustomizer == null)
            {
                GameObject go = new GameObject("BlockCustomizer");
                blockCustomizer = go.AddComponent<BlockCustomizer>();
            }
        }

        if (buildingExecutor != null && blockPrefab != null)
        {
            buildingExecutor.blockPrefab = blockPrefab;
        }
    }
}