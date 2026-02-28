using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingExecutor : MonoBehaviour
{
    public static BuildingExecutor Instance { get; private set; }

    [Header("方块预制体")]
    public GameObject BlockPrefab;

    // key: 地图坐标 (x,y)
    private Dictionary<Vector2Int, BuildingSession> _sessions;
    private Vector2Int? _activeSessionKey;
    private bool _isBuilding = false;

    public event Action<BlueprintData, Vector2Int> OnBuildingStarted;
    public event Action<BlueprintData, Vector2Int> OnBuildingCompleted;
    public event Action<float, Vector2Int> OnBuildingProgress;
    public event Action<string> OnBuildingError;
    public event Action<BlueprintData, Vector2Int> OnBuildingPaused;
    public event Action<BlueprintData, Vector2Int> OnBuildingResumed;

    private class BuildingSession
    {
        public BlueprintData blueprint;
        public Vector2Int mapPosition;
        public GameObject buildingParent;
        public Dictionary<Vector3Int, GameObject> builtBlocks = new Dictionary<Vector3Int, GameObject>();
        public int nextBlockIndex = 0;
        public bool isCompleted = false;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _sessions = new Dictionary<Vector2Int, BuildingSession>();
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
            OnBuildingError?.Invoke("Blueprint data is null");
            return false;
        }
        if (!BuildingLevelManager.Instance.CanBuildWithLevel(blueprint.RequiredLevel))
        {
            OnBuildingError?.Invoke("Insufficient level to build");
            return false;
        }
        return true;
    }

    public bool HasPendingSession(Vector2Int mapPosition)
    {
        return _sessions.TryGetValue(mapPosition, out var s) && !s.isCompleted;
    }

    public float GetSessionProgress(Vector2Int mapPosition)
    {
        if (!_sessions.TryGetValue(mapPosition, out var s)) return 0f;
        if (s.blueprint.Blocks.Count == 0) return 0f;
        return (float)s.nextBlockIndex / s.blueprint.Blocks.Count;
    }

    /// <summary>
    /// 开始建造。同一坐标已有未完成任务则继续，否则新建。
    /// mapPosition 为二维地图坐标，世界 Y 固定为 0。
    /// </summary>
    public void StartBuilding(BlueprintData blueprint, Vector2Int mapPosition)
    {
        if (_isBuilding)
        {
            OnBuildingError?.Invoke("Building in progress, please wait");
            return;
        }
        if (!CheckCanBuild(blueprint)) return;

        BuildingSession session;

        if (_sessions.TryGetValue(mapPosition, out session) && !session.isCompleted)
        {
            OnBuildingResumed?.Invoke(blueprint, mapPosition);
        }
        else
        {
            session = new BuildingSession
            {
                blueprint      = blueprint,
                mapPosition    = mapPosition,
                nextBlockIndex = 0,
                isCompleted    = false
            };
            // 计算建筑物中心点偏移量，使建筑物中心点与目标位置对齐
            float centerOffsetX = (blueprint.Width - 1) * blueprint.BlockSpacing / 2f;
            float centerOffsetZ = (blueprint.Depth - 1) * blueprint.BlockSpacing / 2f;
            Vector3 worldPos = new Vector3(mapPosition.x - centerOffsetX, 0f, mapPosition.y - centerOffsetZ);
            session.buildingParent = new GameObject($"Building_{blueprint.BlueprintName}_{mapPosition.x}_{mapPosition.y}");
            session.buildingParent.transform.position = worldPos;
            _sessions[mapPosition] = session;
            OnBuildingStarted?.Invoke(blueprint, mapPosition);
        }

        _activeSessionKey = mapPosition;
        _isBuilding = true;
        StartCoroutine(BuildCoroutine(session));
    }

    private IEnumerator BuildCoroutine(BuildingSession session)
    {
        int total = session.blueprint.Blocks.Count;

        while (session.nextBlockIndex < total)
        {
            BlockData blockData = session.blueprint.Blocks[session.nextBlockIndex];

            // 每个方块单独消耗 1 个对应材料
            if (!MaterialInventory.Instance.ConsumeMaterial(blockData.MaterialType, 1))
            {
                _isBuilding = false;
                _activeSessionKey = null;
                OnBuildingPaused?.Invoke(session.blueprint, session.mapPosition);
                OnBuildingError?.Invoke($"Insufficient materials: Missing {blockData.MaterialType}, building paused");
                yield break;
            }

            // 计算方块的 Y 坐标，使最底层方块的底部与地面平齐
            // 假设方块的原点在中心，高度为 1 单位
            float yPos = blockData.Y * session.blueprint.BlockSpacing + 0.5f;
            Vector3 localPos = new Vector3(
                blockData.X * session.blueprint.BlockSpacing,
                yPos,
                blockData.Z * session.blueprint.BlockSpacing);

            GameObject block = CreateBlock(blockData, localPos, session.buildingParent.transform);
            if (block != null)
                session.builtBlocks[new Vector3Int(blockData.X, blockData.Y, blockData.Z)] = block;

            session.nextBlockIndex++;
            OnBuildingProgress?.Invoke((float)session.nextBlockIndex / total, session.mapPosition);

            yield return new WaitForSeconds(session.blueprint.BuildDelay);
        }

        session.isCompleted = true;
        _isBuilding = false;
        _activeSessionKey = null;
        OnBuildingCompleted?.Invoke(session.blueprint, session.mapPosition);
    }

    private GameObject CreateBlock(BlockData blockData, Vector3 localPosition, Transform parent)
    {
        if (BlockPrefab == null) { Debug.LogError("方块预制体未设置"); return null; }
        GameObject block = Instantiate(BlockPrefab, parent);
        block.transform.localPosition = localPosition;
        BlockController bc = block.GetComponent<BlockController>() ?? block.AddComponent<BlockController>();
        bc.Initialize(blockData);
        return block;
    }

    public void ClearBuilding(Vector2Int mapPosition)
    {
        if (_sessions.TryGetValue(mapPosition, out var session))
        {
            if (session.buildingParent != null) Destroy(session.buildingParent);
            _sessions.Remove(mapPosition);
        }
        if (_activeSessionKey == mapPosition)
        {
            StopAllCoroutines();
            _isBuilding = false;
            _activeSessionKey = null;
        }
    }

    public void ClearAllBuildings()
    {
        StopAllCoroutines();
        foreach (var s in _sessions.Values)
            if (s.buildingParent != null) Destroy(s.buildingParent);
        _sessions.Clear();
        _isBuilding = false;
        _activeSessionKey = null;
    }

    public bool IsBuilding() => _isBuilding;
    public Vector2Int? GetActiveSessionKey() => _activeSessionKey;
    public BlueprintData GetCurrentBlueprint() =>
        _activeSessionKey.HasValue && _sessions.TryGetValue(_activeSessionKey.Value, out var cur)
            ? cur.blueprint : null;
}
