using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    [SerializeField]
    private GameObject snakeGameObject;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Testing trigger system...");
            
            // 查找蛇控制器
            SnakeController snakeController = snakeGameObject != null ? snakeGameObject.GetComponent<SnakeController>() : null;
            Debug.Log("SnakeController reference: " + (snakeController != null ? "Found" : "Not Found"));
            
            // 查找蛇头触发器
            SnakeHeadTrigger headTrigger = FindObjectOfType<SnakeHeadTrigger>();
            Debug.Log("SnakeHeadTrigger: " + (headTrigger != null ? "Found" : "Not Found"));
            
            if (headTrigger != null)
            {
                Debug.Log("SnakeHeadTrigger has SnakeController: " + (headTrigger.GetComponentInParent<SnakeController>() != null ? "Yes" : "No"));
            }
        }
    }
}
