using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting", menuName = "Game/GameSetting", order = 1)]
public class GameSettingScriptableObject : ScriptableObject
{
    [Header("游戏设置")]
    [Range(0.02f, 0.5f)]
    [Tooltip("移动间隔")]
    public float moveInterval = 0.2f;

    [Range(1, 10)]
    [Tooltip("游戏边界")]
    public int boundary = 4;

    [Header("预制体设置")]
    [Tooltip("蛇身预制体")]
    public GameObject bodyPrefab;

    [Tooltip("食物预制体")]
    public GameObject foodPrefab;

    [Tooltip("边界预制体")]
    public GameObject boundaryPrefab;

    [Header("蛇身设置")]
    [Range(0.5f, 2.0f)]
    [Tooltip("最大身体尺寸（第一个身体）")]
    public float maxBodySize = 0.8f;

    [Range(0.01f, 0.5f)]
    [Tooltip("尺寸变化间隔")]
    public float sizeDecrement = 0.1f;

    [Range(0.1f, 1.0f)]
    [Tooltip("最小身体尺寸")]
    public float minBodySize = 0.4f;

    [Header("标签设置")]
    [Tooltip("边界标签")]
    public string boundaryTag = "Boundary";

    [Tooltip("食物标签")]
    public string foodTag = "Food";

    [Tooltip("蛇身标签")]
    public string bodyTag = "Body";
}
