using UnityEngine;

public class SnakeHeadTrigger : MonoBehaviour
{
    private SnakeController snakeController;
    
    private void Awake()
    {
        // 自动查找父层级中的SnakeController组件
        snakeController = GetComponentInParent<SnakeController>();
        
        if (snakeController == null)
        {
            Debug.LogError("SnakeController not found in parent hierarchy!");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (snakeController != null)
        {
            snakeController.HandleTriggerEnter(other);
        }
    }
}
