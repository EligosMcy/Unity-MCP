using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理命名空间

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private Vector2 xRange = new Vector2(-8, 8);
    [SerializeField] private Vector2 yRange = new Vector2(-4, 4);

    // 小球大小范围和缩小速度
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private float shrinkSpeed = 0.5f; // 加快消失速度

    // 生成数量控制
    [SerializeField] private int maxSpawnCount = 3; // 每次最多生成几个
    [SerializeField] private float multiSpawnChance = 0.3f; // 30%概率一次生成多个

    // 难度控制
    [SerializeField] private float minSpawnInterval = 0.2f;
    [SerializeField] private float difficultyUpdateInterval = 20.0f; // 20秒调整一次难度
    [SerializeField] private float difficultyStep = 0.05f; // 每次减少0.05秒间隔
    [SerializeField] private int penaltyScore = 50; // 错误点击扣分
    [SerializeField] private int maxMisses = 10; // 最大错误次数

    // 统计数据
    public int TotalClicks { get; private set; }
    public int HitCount { get; private set; }
    public float SuccessRate => TotalClicks > 0 ? (float)HitCount / TotalClicks * 100f : 0f;
    public int MissCount { get; private set; } // 错误点击次数
    
    // 积分系统
    public int Score { get; private set; }

    private float _timer;
    private float _difficultyTimer;
    private Queue<Ball> _ballPool = new Queue<Ball>();

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

    private void Update()
    {
        _timer += Time.deltaTime;
        
        // 难度随时间增加：每20秒减少生成间隔
        _difficultyTimer += Time.deltaTime;
        if (_difficultyTimer >= difficultyUpdateInterval)
        {
            _difficultyTimer = 0f;
            if (spawnInterval > minSpawnInterval)
            {
                spawnInterval -= difficultyStep;
                spawnInterval = Mathf.Max(spawnInterval, minSpawnInterval);
            }
        }

        if (_timer >= spawnInterval)
        {
            SpawnBalls();
            _timer = 0f;
        }
    }

    private void OnGUI()
    {
        // 简单的UI显示
        GUILayout.BeginArea(new Rect(10, 10, 300, 250));
        GUI.skin.label.fontSize = 20;
        GUILayout.Label($"Total Clicks: {TotalClicks}");
        GUILayout.Label($"Hits: {HitCount}");
        GUILayout.Label($"Misses: {MissCount}/{maxMisses}");
        GUILayout.Label($"Success Rate: {SuccessRate:F1}%");
        GUILayout.Label($"Score: {Score}");
        GUILayout.Label($"Spawn Rate: {spawnInterval:F2}s");
        GUILayout.EndArea();
    }

    public void RegisterClick(bool isHit)
    {
        TotalClicks++;
        if (isHit)
        {
            HitCount++;
        }
        else
        {
            // 错误点击处理
            MissCount++;
            AddScore(-penaltyScore); // 扣分
            
            // 检查是否游戏结束
            if (MissCount >= maxMisses)
            {
                RestartGame();
            }
        }
    }

    public void AddScore(int amount)
    {
        Score += amount;
    }

    private void RestartGame()
    {
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SpawnBalls()
    {
        // 决定本次生成的数量
        int count = 1;
        if (Random.value < multiSpawnChance)
        {
            count = Random.Range(1, maxSpawnCount + 1);
        }

        for (int i = 0; i < count; i++)
        {
            SpawnSingleBall();
        }
    }

    private void SpawnSingleBall()
    {
        if (ballPrefab == null) return;

        Ball ball = GetBallFromPool();

        float x = Random.Range(xRange.x, xRange.y);
        float y = Random.Range(yRange.x, yRange.y);
        Vector3 pos = new Vector3(x, y, 0);

        ball.transform.position = pos;
        ball.transform.rotation = Quaternion.identity;
        ball.gameObject.SetActive(true);
        
        // 初始化大小和缩小速度
        ball.Init(minScale, maxScale, shrinkSpeed);
    }

    private Ball GetBallFromPool()
    {
        if (_ballPool.Count > 0)
        {
            return _ballPool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(ballPrefab);
            return obj.GetComponent<Ball>();
        }
    }

    public void ReturnBall(Ball ball)
    {
        ball.gameObject.SetActive(false);
        _ballPool.Enqueue(ball);
    }
}