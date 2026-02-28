using UnityEngine;

[CreateAssetMenu(fileName = "BuildingLevelData", menuName = "Building/BuildingLevelData", order = 1)]
public class BuildingLevelData : ScriptableObject
{
    [Header("等级信息")]
    public int Level;

    [Header("技能等级要求")]
    public int WoodworkingLevel;
    public int ConstructionLevel;

    [Header("解锁描述")]
    [TextArea]
    public string Description;
}