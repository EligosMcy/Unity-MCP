using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting", menuName = "Game/GameSetting", order = 1)]
public class GameSettingScriptableObject : ScriptableObject
{
    [Header("游戏设置")]
    [Range(0.02f, 0.5f)]
    [Tooltip("移动间隔")]
    public float MoveInterval = 0.2f;

    [Range(1, 10)]
    [Tooltip("游戏边界")]
    public int Boundary = 4;

    [Header("预制体设置")]
    [Tooltip("蛇身预制体")]
    public GameObject BodyPrefab;

    [Tooltip("食物预制体")]
    public GameObject FoodPrefab;

    [Tooltip("大食物预制体")]
    public GameObject BigFoodPrefab;

    [Tooltip("边界预制体")]
    public GameObject BoundaryPrefab;

    [Header("蛇身设置")]
    [Range(0.5f, 2.0f)]
    [Tooltip("最大身体尺寸（第一个身体）")]
    public float MaxBodySize = 0.8f;

    [Range(0.01f, 0.5f)]
    [Tooltip("尺寸变化间隔")]
    public float SizeDecrement = 0.1f;

    [Range(0.1f, 1.0f)]
    [Tooltip("最小身体尺寸")]
    public float MinBodySize = 0.4f;

    [Header("分数设置")]
    [Tooltip("小食物分数")]
    public int SmallFoodScore = 1;

    [Tooltip("大食物分数")]
    public int BigFoodScore = 5;

    [Header("标签设置")]
    [Tooltip("边界标签")]
    public string BoundaryTag = "Boundary";

    [Tooltip("食物标签")]
    public string FoodTag = "Food";

    public string BigFoodTag = "BigFood";

    [Tooltip("蛇身标签")]
    public string BodyTag = "Body";

    [Header("动画设置")]
    [Range(0.1f, 1.0f)]
    [Tooltip("身体变色动画持续时间")]
    public float ColorChangeDuration = 0.2f;

    [Tooltip("变色目标颜色")]
    public Color TargetColor = Color.yellow;

    [Tooltip("原始身体颜色")]
    public Color OriginalColor = Color.white;
}
