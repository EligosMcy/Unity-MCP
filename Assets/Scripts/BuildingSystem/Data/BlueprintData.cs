using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueprintData", menuName = "Building/BlueprintData", order = 2)]
public class BlueprintData : ScriptableObject
{
    [Header("蓝图信息")]
    public string blueprintName;

    [Header("区域大小")]
    public int width;
    public int height;
    public int depth;

    [Header("方块数据")]
    public List<BlockData> blocks;

    [Header("材料需求统计（可序列化）")]
    public List<MaterialRequirementEntry> materialRequirementList = new List<MaterialRequirementEntry>();

    [Header("建筑等级要求")]
    public BuildingLevelData requiredLevel;

    [Header("描述")]
    [TextArea]
    public string description;

    // 运行时缓存，由 materialRequirementList 构建
    private Dictionary<MaterialType, int> _materialRequirementsCache;

    public Dictionary<MaterialType, int> materialRequirements
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
        if (materialRequirementList == null) return;
        foreach (var entry in materialRequirementList)
            _materialRequirementsCache[entry.materialType] = entry.amount;
    }

    public void CalculateMaterialRequirements()
    {
        materialRequirementList = new List<MaterialRequirementEntry>();
        var temp = new Dictionary<MaterialType, int>();

        foreach (var block in blocks)
        {
            if (temp.ContainsKey(block.materialType))
                temp[block.materialType]++;
            else
                temp[block.materialType] = 1;
        }

        foreach (var kv in temp)
            materialRequirementList.Add(new MaterialRequirementEntry { materialType = kv.Key, amount = kv.Value });

        RebuildCache();
    }

    public int GetTotalBlockCount()
    {
        return blocks != null ? blocks.Count : 0;
    }

    public int GetMaterialRequirement(MaterialType materialType)
    {
        if (materialRequirements.TryGetValue(materialType, out int amount))
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
