using UnityEngine;
using System.Collections.Generic;

public class PathfindingVisualizer : MonoBehaviour
{
    [SerializeField]
    private AutoPathfinding _autoPathfinding;

    [SerializeField]
    private FoodManager _foodManager;

    [SerializeField]
    private SnakeController _snakeController;

    [SerializeField]
    private bool _showWalkableGrid = true;

    [SerializeField]
    private bool _showPath = true;

    [SerializeField]
    private bool _showUnreachable = true;

    [SerializeField]
    private bool _showFoodSpawnPositions = true;

    [SerializeField]
    private Color _walkableColor = new Color(0, 1, 0, 0.3f);

    [SerializeField]
    private Color _pathColor = new Color(1, 1, 0, 0.8f);

    [SerializeField]
    private Color _unreachableColor = new Color(1, 0, 0, 0.8f);

    [SerializeField]
    private Color _foodSpawnColor = new Color(0, 0, 1, 0.5f);

    private float _boundary = 4.5f;
    private List<GameObject> _bodyParts;
    private Vector3 _lastUnreachablePos = Vector3.zero;
    private bool _wasUnreachable = false;

    private void Awake()
    {
        // 获取边界值
        GameSettingScriptableObject gameSetting = Resources.Load<GameSettingScriptableObject>("GameSettings/DefaultGameSetting");
        if (gameSetting != null)
        {
            _boundary = gameSetting.boundary;
        }
    }

    private void Start()
    {
        // 如果没有手动分配引用，尝试自动查找
        if (_autoPathfinding == null)
        {
            _autoPathfinding = FindObjectOfType<AutoPathfinding>();
        }
        if (_foodManager == null)
        {
            _foodManager = FindObjectOfType<FoodManager>();
        }
        if (_snakeController == null)
        {
            _snakeController = FindObjectOfType<SnakeController>();
        }
    }

    private void Update()
    {
        // 更新身体段引用
        if (_snakeController != null)
        {
            var bodyPartsField = typeof(SnakeController).GetField("_bodyParts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (bodyPartsField != null)
            {
                _bodyParts = bodyPartsField.GetValue(_snakeController) as List<GameObject>;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_autoPathfinding == null || _foodManager == null)
            return;

        GameObject food = _foodManager.GetFood();
        if (food == null)
            return;

        Vector3 foodPos = food.transform.position;
        Vector3 headPos = Vector3.zero;

        // 获取蛇头位置
        if (_snakeController != null)
        {
            var headField = typeof(SnakeController).GetField("_head", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (headField != null)
            {
                GameObject head = headField.GetValue(_snakeController) as GameObject;
                if (head != null)
                {
                    headPos = head.transform.localPosition;
                }
            }
        }

        // 绘制可行走的方格
        if (_showWalkableGrid)
        {
            DrawWalkableGrid();
        }

        // 绘制食物可以生成的所有位置
        if (_showFoodSpawnPositions)
        {
            DrawFoodSpawnPositions();
        }

        // 计算并绘制路径
        if (_showPath)
        {
            List<Vector3> path = CalculatePath(headPos, foodPos);
            if (path.Count > 0)
            {
                DrawPath(path);
                _wasUnreachable = false;
            }
            else
            {
                // 无法找到路径
                if (_showUnreachable)
                {
                    DrawUnreachableArea(headPos);
                }
            }
        }
    }

    // 绘制可行走的方格
    private void DrawWalkableGrid()
    {
        for (int x = Mathf.FloorToInt(-_boundary); x <= Mathf.FloorToInt(_boundary); x++)
        {
            for (int z = Mathf.FloorToInt(-_boundary); z <= Mathf.FloorToInt(_boundary); z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                if (!IsPositionObstacle(pos))
                {
                    Gizmos.color = _walkableColor;
                    Gizmos.DrawCube(pos, Vector3.one * 0.95f);
                }
            }
        }
    }

    // 计算路径
    private List<Vector3> CalculatePath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] directions = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        Queue<Vector3> queue = new Queue<Vector3>();
        queue.Enqueue(startPos);
        visited.Add(Vector3Int.FloorToInt(startPos));
        Dictionary<Vector3, Vector3> parent = new Dictionary<Vector3, Vector3>();

        while (queue.Count > 0)
        {
            Vector3 currentPos = queue.Dequeue();

            if (Vector3.Distance(currentPos, targetPos) < 0.5f)
            {
                return ReconstructPath(startPos, currentPos, parent);
            }

            foreach (Vector3 dir in directions)
            {
                Vector3 nextPos = currentPos + dir;
                Vector3Int nextPosInt = Vector3Int.FloorToInt(nextPos);

                if (!visited.Contains(nextPosInt) && !IsPositionObstacle(nextPos))
                {
                    queue.Enqueue(nextPos);
                    visited.Add(nextPosInt);
                    parent[nextPos] = currentPos;
                }
            }
        }

        // 无法找到路径，返回空列表
        return new List<Vector3>();
    }

    // 重建路径
    private List<Vector3> ReconstructPath(Vector3 startPos, Vector3 endPos, Dictionary<Vector3, Vector3> parent)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 currentPos = endPos;

        while (currentPos != startPos)
        {
            path.Add(currentPos);
            if (parent.ContainsKey(currentPos))
            {
                currentPos = parent[currentPos];
            }
            else
            {
                break;
            }
        }

        path.Reverse();
        return path;
    }

    // 绘制路径
    private void DrawPath(List<Vector3> path)
    {
        Gizmos.color = _pathColor;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pos = path[i];
            Gizmos.DrawCube(pos, Vector3.one * 0.8f);

            // 绘制连接线
            if (i > 0)
            {
                Gizmos.DrawLine(path[i - 1], pos);
            }
        }
    }

    // 绘制无法到达的区域
    private void DrawUnreachableArea(Vector3 headPos)
    {
        Vector3[] directions = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        Queue<Vector3> queue = new Queue<Vector3>();
        queue.Enqueue(headPos);
        visited.Add(Vector3Int.FloorToInt(headPos));

        Vector3 farthestPos = headPos;
        float maxDistance = 0;

        while (queue.Count > 0)
        {
            Vector3 currentPos = queue.Dequeue();

            foreach (Vector3 dir in directions)
            {
                Vector3 nextPos = currentPos + dir;
                Vector3Int nextPosInt = Vector3Int.FloorToInt(nextPos);

                if (!visited.Contains(nextPosInt) && !IsPositionObstacle(nextPos))
                {
                    queue.Enqueue(nextPos);
                    visited.Add(nextPosInt);

                    // 计算距离蛇头的距离
                    float distance = Vector3.Distance(nextPos, headPos);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestPos = nextPos;
                    }
                }
            }
        }

        _lastUnreachablePos = farthestPos;
        _wasUnreachable = true;

        // 绘制所有可达的区域
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        foreach (Vector3Int posInt in visited)
        {
            Vector3 pos = new Vector3(posInt.x, 0, posInt.z);
            Gizmos.DrawCube(pos, Vector3.one * 0.95f);
        }

        // 绘制最远的位置
        Gizmos.color = _unreachableColor;
        Gizmos.DrawCube(farthestPos, Vector3.one * 1.2f);

        // 在控制台输出信息
        if (!_wasUnreachable)
        {
            Debug.LogWarning("无法找到到达食物的路径！");
            Debug.LogWarning("蛇头位置: " + headPos);
            Debug.LogWarning("食物位置: " + _foodManager.GetFood().transform.position);
            Debug.LogWarning("最远可达位置: " + farthestPos);
        }
    }

    // 检查位置是否是障碍物
    private bool IsPositionObstacle(Vector3 position)
    {
        if (Mathf.Abs(position.x) >= _boundary || Mathf.Abs(position.z) >= _boundary)
        {
            return true;
        }

        if (_bodyParts != null)
        {
            foreach (GameObject bodyPart in _bodyParts)
            {
                if (Vector3.Distance(position, bodyPart.transform.localPosition) < 0.5f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // 设置边界
    public void SetBoundary(float boundary)
    {
        _boundary = boundary;
    }

    // 绘制食物可以生成的所有位置
    private void DrawFoodSpawnPositions()
    {
        // 获取蛇头位置
        Vector3 headPos = Vector3.zero;
        if (_snakeController != null)
        {
            var headField = typeof(SnakeController).GetField("_head", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (headField != null)
            {
                GameObject head = headField.GetValue(_snakeController) as GameObject;
                if (head != null)
                {
                    headPos = head.transform.localPosition;
                }
            }
        }

        // 遍历所有可能的方格位置
        for (int x = Mathf.FloorToInt(-_boundary); x <= Mathf.FloorToInt(_boundary); x++)
        {
            for (int z = Mathf.FloorToInt(-_boundary); z <= Mathf.FloorToInt(_boundary); z++)
            {
                Vector3 pos = new Vector3(x, 0, z);

                // 检查是否可以生成食物（不在蛇身上，不在边界上）
                if (IsValidFoodSpawnPosition(pos, headPos))
                {
                    Gizmos.color = _foodSpawnColor;
                    Gizmos.DrawCube(pos, Vector3.one * 0.7f);
                }
            }
        }
    }

    // 检查位置是否可以生成食物
    private bool IsValidFoodSpawnPosition(Vector3 position, Vector3 headPos)
    {
        // 检查是否在边界上
        if (Mathf.Abs(position.x) >= _boundary || Mathf.Abs(position.z) >= _boundary)
        {
            return false;
        }

        // 检查是否与蛇头重叠
        if (Vector3.Distance(position, headPos) < 0.5f)
        {
            return false;
        }

        // 检查是否与蛇身重叠
        if (_bodyParts != null)
        {
            foreach (GameObject bodyPart in _bodyParts)
            {
                if (Vector3.Distance(position, bodyPart.transform.localPosition) < 0.5f)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
