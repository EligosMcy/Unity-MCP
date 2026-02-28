using UnityEngine;

public class BuildingSystemManager : MonoBehaviour
{
    public static BuildingSystemManager Instance { get; private set; }

    [Header("系统引用")]
    public MaterialInventory MaterialInventory;
    public BuildingLevelManager BuildingLevelManager;
    public BuildingExecutor BuildingExecutor;
    public BlockCustomizer BlockCustomizer;
    public BuildingUI BuildingUI;

    [Header("方块预制体")]
    public GameObject BlockPrefab;

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
}