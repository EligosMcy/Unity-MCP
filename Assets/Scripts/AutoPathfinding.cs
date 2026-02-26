using UnityEngine;
using System.Collections.Generic;

public class AutoPathfinding : MonoBehaviour
{
    [SerializeField]
    private FoodManager _foodManager;

    [SerializeField]
    private SnakeController _snakeController;

    // 游戏区域的边界
    private float _boundary = 4.5f;

    // 蛇身的游戏对象列表
    private List<GameObject> _bodyParts;

    // 移动方向
    private Vector3 _direction = Vector3.right;

    // 初始化
    public void Initialize(FoodManager foodManager, SnakeController snakeController, float boundary)
    {
        _foodManager = foodManager;
        _snakeController = snakeController;
        _boundary = boundary;
    }

    // 更新身体段引用
    public void UpdateBodyPartsReference(List<GameObject> bodyParts)
    {
        _bodyParts = bodyParts;
    }

    // 计算到食物的路径
    public Vector3 CalculatePath(Vector3 headPos)
    {
        // 获取食物位置
        GameObject food = _foodManager.GetActiveFood();
        if (food == null)
            return _direction;

        Vector3 foodPos = food.transform.position;

        // 使用BFS算法计算最短路径
        Vector3 targetDirection = BFS(headPos, foodPos);
        if (targetDirection != Vector3.zero)
        {
            _direction = targetDirection;
        }

        return _direction;
    }

    // 广度优先搜索算法
    private Vector3 BFS(Vector3 startPos, Vector3 targetPos)
    {
        // 可能的移动方向
        Vector3[] directions = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

        // 已访问的位置
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        // 队列，用于BFS
        Queue<Vector3> queue = new Queue<Vector3>();
        queue.Enqueue(startPos);
        visited.Add(Vector3Int.FloorToInt(startPos));

        // 父节点映射，用于回溯路径
        Dictionary<Vector3, Vector3> parent = new Dictionary<Vector3, Vector3>();

        // BFS搜索
        while (queue.Count > 0)
        {
            Vector3 currentPos = queue.Dequeue();

            // 到达目标
            if (Vector3.Distance(currentPos, targetPos) < 0.5f)
            {
                // 回溯路径，找到第一步移动方向
                return GetFirstStepDirection(startPos, currentPos, parent);
            }

            // 尝试所有可能的移动方向
            foreach (Vector3 dir in directions)
            {
                Vector3 nextPos = currentPos + dir;
                Vector3Int nextPosInt = Vector3Int.FloorToInt(nextPos);

                // 检查是否是有效位置
                if (!visited.Contains(nextPosInt) && !IsPositionObstacle(nextPos))
                {
                    queue.Enqueue(nextPos);
                    visited.Add(nextPosInt);
                    parent[nextPos] = currentPos;
                }
            }
        }

        // 如果没有找到路径，返回当前方向
        return _direction;
    }

    // 获取第一步移动方向
    private Vector3 GetFirstStepDirection(Vector3 startPos, Vector3 endPos, Dictionary<Vector3, Vector3> parent)
    {
        // 回溯路径
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

        // 反转路径，从起点开始
        path.Reverse();

        // 返回第一步移动方向
        if (path.Count > 0)
        {
            Vector3 firstStep = path[0];
            return (firstStep - startPos).normalized;
        }

        return _direction;
    }

    // 检查位置是否是障碍物
    private bool IsPositionObstacle(Vector3 position)
    {
        // 检查边界
        if (Mathf.Abs(position.x) >= _boundary || Mathf.Abs(position.z) >= _boundary)
        {
            return true;
        }

        // 检查自身碰撞
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
}
