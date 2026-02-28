using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueprintData", menuName = "Building/BlueprintData", order = 2)]
public class BlueprintData : ScriptableObject
{
    [Header("蓝图信息")]
    public string BlueprintName;

    [Header("区域大小")]
    public int Width;
    public int Height;
    public int Depth;

    [Header("方块数据")]
    public List<BlockData> Blocks;

    [Header("材料需求统计（可序列化）")]
    public List<MaterialRequirementEntry> MaterialRequirementList = new List<MaterialRequirementEntry>();

    [Header("建筑等级要求")]
    public BuildingLevelData RequiredLevel;

    [Header("描述")]
    [TextArea]
    public string Description;

    // 运行时缓存，由 MaterialRequirementList 构建
    private Dictionary<MaterialType, int> _materialRequirementsCache;

    public Dictionary<MaterialType, int> MaterialRequirements
    {
        get
        {
            if (_materialRequirementsCache == null)
                RebuildCache();
            return _materialRequirementsCache;
        }
    }

    private void OnEnable()
    {
        RebuildCache();
    }

    private void RebuildCache()
    {
        _materialRequirementsCache = new Dictionary<MaterialType, int>();
        if (MaterialRequirementList == null) return;
        foreach (var entry in MaterialRequirementList)
            _materialRequirementsCache[entry.materialType] = entry.amount;
    }

    public void CalculateMaterialRequirements()
    {
        MaterialRequirementList = new List<MaterialRequirementEntry>();
        var temp = new Dictionary<MaterialType, int>();

        foreach (var block in Blocks)
        {
            if (temp.ContainsKey(block.materialType))
                temp[block.materialType]++;
            else
                temp[block.materialType] = 1;
        }

        foreach (var kv in temp)
            MaterialRequirementList.Add(new MaterialRequirementEntry { materialType = kv.Key, amount = kv.Value });

        RebuildCache();
    }

    public int GetTotalBlockCount()
    {
        return Blocks != null ? Blocks.Count : 0;
    }

    public int GetMaterialRequirement(MaterialType materialType)
    {
        if (MaterialRequirements.TryGetValue(materialType, out int amount))
            return amount;
        return 0;
    }
}

[Serializable]
public class MaterialRequirementEntry
{
    public MaterialType materialType;
    public int amount;
}
