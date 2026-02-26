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

    [Header("材料需求统计")]
    public Dictionary<MaterialType, int> materialRequirements;

    [Header("建筑等级要求")]
    public BuildingLevelData requiredLevel;

    [Header("描述")]
    [TextArea]
    public string description;

    public void CalculateMaterialRequirements()
    {
        materialRequirements = new Dictionary<MaterialType, int>();

        foreach (var block in blocks)
        {
            if (materialRequirements.ContainsKey(block.materialType))
            {
                materialRequirements[block.materialType]++;
            }
            else
            {
                materialRequirements[block.materialType] = 1;
            }
        }
    }

    public int GetTotalBlockCount()
    {
        return blocks.Count;
    }

    public int GetMaterialRequirement(MaterialType materialType)
    {
        if (materialRequirements.ContainsKey(materialType))
        {
            return materialRequirements[materialType];
        }
        return 0;
    }
}