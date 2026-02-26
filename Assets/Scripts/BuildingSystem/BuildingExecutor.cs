using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingExecutor : MonoBehaviour
{
    public static BuildingExecutor Instance { get; private set; }

    [Header("方块预制体")]
    public GameObject blockPrefab;

    [Header("建造设置")]
    public float blockSpacing = 1.0f;
    public float buildDelay = 0.1f;

    private BlueprintData _currentBlueprint;
    private GameObject _buildingParent;
    private Dictionary<Vector3Int, GameObject> _builtBlocks;
    private bool _isBuilding = false;

    public event Action<BlueprintData> OnBuildingStarted;
    public event Action<BlueprintData> OnBuildingCompleted;
    public event Action<float> OnBuildingProgress;
    public event Action<string> OnBuildingError;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _builtBlocks = new Dictionary<Vector3Int, GameObject>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CheckCanBuild(BlueprintData blueprint)
    {
        if (blueprint == null)
        {
            OnBuildingError?.Invoke("蓝图数据为空");
            return false;
        }

        if (!BuildingLevelManager.Instance.CanBuildWithLevel(blueprint.requiredLevel))
        {
            OnBuildingError?.Invoke("等级不足，无法建造");
            return false;
        }

        return true;
    }

    public bool CheckMaterialsSufficient(BlueprintData blueprint)
    {
        if (blueprint == null) return false;

        return MaterialInventory.Instance.CheckMaterialSufficient(blueprint.materialRequirements);
    }

    public void StartBuilding(BlueprintData blueprint, Vector3 position)
    {
        if (_isBuilding)
        {
            OnBuildingError?.Invoke("正在建造中，请稍候");
            return;
        }

        if (!CheckCanBuild(blueprint))
        {
            return;
        }

        _currentBlueprint = blueprint;
        _isBuilding = true;

        CreateBuildingParent(position);
        OnBuildingStarted?.Invoke(blueprint);

        StartCoroutine(BuildCoroutine());
    }

    private void CreateBuildingParent(Vector3 position)
    {
        if (_buildingParent != null)
        {
            Destroy(_buildingParent);
        }

        _buildingParent = new GameObject($"Building_{_currentBlueprint.blueprintName}");
        _buildingParent.transform.position = position;
    }

    private System.Collections.IEnumerator BuildCoroutine()
    {
        int totalBlocks = _currentBlueprint.blocks.Count;
        int builtBlocks = 0;

        foreach (var blockData in _currentBlueprint.blocks)
        {
            Vector3 worldPosition = CalculateBlockPosition(blockData);
            GameObject block = CreateBlock(blockData, worldPosition);

            if (block != null)
            {
                Vector3Int gridPosition = new Vector3Int(blockData.x, blockData.y, blockData.z);
                _builtBlocks[gridPosition] = block;
            }

            builtBlocks++;
            OnBuildingProgress?.Invoke((float)builtBlocks / totalBlocks);

            yield return new WaitForSeconds(buildDelay);
        }

        _isBuilding = false;
        OnBuildingCompleted?.Invoke(_currentBlueprint);
    }

    private Vector3 CalculateBlockPosition(BlockData blockData)
    {
        float x = blockData.x * blockSpacing;
        float y = blockData.y * blockSpacing;
        float z = blockData.z * blockSpacing;

        return new Vector3(x, y, z);
    }

    private GameObject CreateBlock(BlockData blockData, Vector3 position)
    {
        if (blockPrefab == null)
        {
            Debug.LogError("方块预制体未设置");
            return null;
        }

        GameObject block = Instantiate(blockPrefab, _buildingParent.transform);
        block.transform.localPosition = position;

        BlockController blockController = block.GetComponent<BlockController>();
        if (blockController == null)
        {
            blockController = block.AddComponent<BlockController>();
        }

        blockController.Initialize(blockData);

        return block;
    }

    public void ClearBuilding()
    {
        if (_buildingParent != null)
        {
            Destroy(_buildingParent);
            _buildingParent = null;
        }

        _builtBlocks.Clear();
        _isBuilding = false;
    }

    public GameObject GetBlockAt(Vector3Int gridPosition)
    {
        if (_builtBlocks.ContainsKey(gridPosition))
        {
            return _builtBlocks[gridPosition];
        }
        return null;
    }

    public void UpdateBlockColor(Vector3Int gridPosition, Color newColor)
    {
        GameObject block = GetBlockAt(gridPosition);
        if (block != null)
        {
            BlockController blockController = block.GetComponent<BlockController>();
            if (blockController != null)
            {
                blockController.SetColor(newColor);
            }
        }
    }

    public bool IsBuilding()
    {
        return _isBuilding;
    }

    public BlueprintData GetCurrentBlueprint()
    {
        return _currentBlueprint;
    }
}