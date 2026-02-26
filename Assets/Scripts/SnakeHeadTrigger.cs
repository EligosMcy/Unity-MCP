using UnityEngine;

public class SnakeHeadTrigger : MonoBehaviour
{
    private SnakeController _snakeController;
    
    private void Awake()
    {
        // 自动查找父层级中的SnakeController组件
        _snakeController = GetComponentInParent<SnakeController>();
        
        if (_snakeController == null)
        {
            Debug.LogError("SnakeController not found in parent hierarchy!");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_snakeController != null)
        {
            _snakeController.HandleTriggerEnter(other);
        }
    }
}
