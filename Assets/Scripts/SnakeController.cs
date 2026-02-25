using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    // 游戏状态枚举
    private enum GameState { Start, Playing, GameOver }
    
    // 当前游戏状态
    private GameState currentState = GameState.Start;
    
    // 蛇的移动速度
    public float moveSpeed = 1.0f;
    
    // 移动方向
    private Vector3 direction = Vector3.right;
    
    // 蛇身的游戏对象列表
    private List<GameObject> bodyParts = new List<GameObject>();
    
    // 蛇头的引用
    public GameObject head;
    
    // 移动计时器
    private float moveTimer = 0.0f;
    
    // 移动间隔
    private float moveInterval = 0.2f;
    
    // 游戏是否结束
    private bool gameOver = false;
    
    // 得分
    private int score = 0;
    
    // 得分文本的引用
    public UnityEngine.UI.Text scoreText;
    
    // 开始按钮的引用
    public UnityEngine.UI.Button startButton;
    
    // 游戏结束文本的引用
    public UnityEngine.UI.Text gameOverText;
    
    // 食物的引用
    private GameObject food;
    
    // 游戏区域的边界
    private float boundary = 4.5f;
    
    // 输入系统
    private PlayerInput playerInput;
    private InputAction moveUpAction;
    private InputAction moveDownAction;
    private InputAction moveLeftAction;
    private InputAction moveRightAction;
    
    void Start()
    {
        // 自动查找head对象
        head = transform.Find("Head").gameObject;
        
        // 初始化蛇身列表，添加初始的身体段
        foreach (Transform child in transform)
        {
            if (child.name != "Head")
            {
                bodyParts.Add(child.gameObject);
            }
        }
        
        // 查找得分文本对象
        GameObject scoreTextObj = GameObject.Find("ScoreText");
        if (scoreTextObj != null)
        {
            scoreText = scoreTextObj.GetComponent<UnityEngine.UI.Text>();
        }
        
        // 查找开始按钮对象
        GameObject startButtonObj = GameObject.Find("StartButton");
        if (startButtonObj != null)
        {
            startButton = startButtonObj.GetComponent<UnityEngine.UI.Button>();
            if (startButton != null)
            {
                startButton.onClick.AddListener(StartGame);
            }
        }
        
        // 创建游戏结束文本对象
        GameObject gameOverTextObj = new GameObject("GameOverText");
        gameOverTextObj.transform.SetParent(GameObject.Find("Canvas").transform);
        gameOverTextObj.transform.localPosition = new Vector3(0, 100, 0);
        gameOverText = gameOverTextObj.AddComponent<UnityEngine.UI.Text>();
        gameOverText.text = "Game Over!\nScore: 0\nClick Start to play again.";
        gameOverText.alignment = TextAnchor.MiddleCenter;
        gameOverText.gameObject.SetActive(false);
        
        // 初始化输入系统
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
        }
        
        // 创建输入动作
        moveUpAction = new InputAction(binding: "<Keyboard>/w");
        moveDownAction = new InputAction(binding: "<Keyboard>/s");
        moveLeftAction = new InputAction(binding: "<Keyboard>/a");
        moveRightAction = new InputAction(binding: "<Keyboard>/d");
        
        // 启用输入动作
        moveUpAction.Enable();
        moveDownAction.Enable();
        moveLeftAction.Enable();
        moveRightAction.Enable();
        
        // 设置输入动作的回调
        moveUpAction.performed += ctx => OnMoveUp();
        moveDownAction.performed += ctx => OnMoveDown();
        moveLeftAction.performed += ctx => OnMoveLeft();
        moveRightAction.performed += ctx => OnMoveRight();
        
        // 更新初始得分显示
        UpdateScoreDisplay();
        
        // 设置游戏状态为Start
        SetGameState(GameState.Start);
    }
    
    // 移动向上
    void OnMoveUp()
    {
        if (currentState == GameState.Playing && direction != Vector3.down)
        {
            direction = Vector3.forward;
        }
    }
    
    // 移动向下
    void OnMoveDown()
    {
        if (currentState == GameState.Playing && direction != Vector3.forward)
        {
            direction = Vector3.back;
        }
    }
    
    // 移动向左
    void OnMoveLeft()
    {
        if (currentState == GameState.Playing && direction != Vector3.right)
        {
            direction = Vector3.left;
        }
    }
    
    // 移动向右
    void OnMoveRight()
    {
        if (currentState == GameState.Playing && direction != Vector3.left)
        {
            direction = Vector3.right;
        }
    }
    
    // 设置游戏状态
    void SetGameState(GameState state)
    {
        currentState = state;
        
        switch (state)
        {
            case GameState.Start:
                // 显示开始按钮
                if (startButton != null)
                {
                    startButton.gameObject.SetActive(true);
                }
                // 隐藏游戏结束文本
                if (gameOverText != null)
                {
                    gameOverText.gameObject.SetActive(false);
                }
                // 暂停游戏
                Time.timeScale = 0;
                break;
                
            case GameState.Playing:
                // 隐藏开始按钮
                if (startButton != null)
                {
                    startButton.gameObject.SetActive(false);
                }
                // 隐藏游戏结束文本
                if (gameOverText != null)
                {
                    gameOverText.gameObject.SetActive(false);
                }
                // 开始游戏
                Time.timeScale = 1;
                // 生成初始食物
                SpawnFood();
                break;
                
            case GameState.GameOver:
                // 显示开始按钮
                if (startButton != null)
                {
                    startButton.gameObject.SetActive(true);
                }
                // 显示游戏结束文本
                if (gameOverText != null)
                {
                    gameOverText.text = "Game Over!\nScore: " + score + "\nClick Start to play again.";
                    gameOverText.gameObject.SetActive(true);
                }
                // 暂停游戏
                Time.timeScale = 0;
                break;
        }
    }
    
    // 开始游戏
    void StartGame()
    {
        // 重置得分
        score = 0;
        UpdateScoreDisplay();
        
        // 重置蛇的位置和长度
        ResetSnake();
        
        // 设置游戏状态为Playing
        SetGameState(GameState.Playing);
    }
    
    // 重置蛇
    void ResetSnake()
    {
        // 清除所有身体段
        foreach (GameObject bodyPart in bodyParts)
        {
            Destroy(bodyPart);
        }
        bodyParts.Clear();
        
        // 重置蛇头位置
        head.transform.localPosition = Vector3.zero;
        
        // 重置移动方向
        direction = Vector3.right;
        
        // 生成初始身体段
        GameObject body1 = Instantiate(head, transform);
        body1.name = "Body1";
        body1.transform.localPosition = new Vector3(-1, 0, 0);
        
        // 为身体段设置材质
        Renderer renderer = body1.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material bodyMaterial = Resources.Load<Material>("Materials/BodyMaterial");
            if (bodyMaterial != null)
            {
                renderer.material = bodyMaterial;
            }
        }
        
        bodyParts.Add(body1);
        
        // 清除食物
        if (food != null)
        {
            Destroy(food);
            food = null;
        }
    }
    
    // 更新得分显示
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    
    void Update()
    {
        // 只有在游戏状态为Playing时才处理移动
        if (currentState == GameState.Playing)
        {
            // 更新移动
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveInterval)
            {
                Move();
                moveTimer = 0.0f;
            }
        }
    }
    
    // 移动蛇
    void Move()
    {
        // 记录蛇头的当前位置
        Vector3 previousPosition = head.transform.localPosition;
        
        // 移动蛇头
        head.transform.localPosition += direction;
        
        // 检查边界碰撞
        if (Mathf.Abs(head.transform.localPosition.x) > boundary || Mathf.Abs(head.transform.localPosition.z) > boundary)
        {
            GameOver();
            return;
        }
        
        // 检查自身碰撞
        foreach (GameObject bodyPart in bodyParts)
        {
            if (Vector3.Distance(head.transform.localPosition, bodyPart.transform.localPosition) < 0.5f)
            {
                GameOver();
                return;
            }
        }
        
        // 检查食物碰撞
        if (food != null && Vector3.Distance(head.transform.position, food.transform.position) < 0.5f)
        {
            CollectFood();
        }
        
        // 移动蛇身
        if (bodyParts.Count > 0)
        {
            for (int i = 0; i < bodyParts.Count; i++)
            {
                Vector3 tempPosition = bodyParts[i].transform.localPosition;
                bodyParts[i].transform.localPosition = previousPosition;
                previousPosition = tempPosition;
            }
        }
    }
    
    // 生成食物
    void SpawnFood()
    {
        // 如果食物不存在，创建一个新的
        if (food == null)
        {
            food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            food.name = "Food";
            food.transform.localScale = new Vector3(1, 1, 1);
            
            // 为食物设置FoodMaterial材质
            Renderer renderer = food.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material foodMaterial = Resources.Load<Material>("Materials/FoodMaterial");
                if (foodMaterial != null)
                {
                    renderer.material = foodMaterial;
                }
            }
        }
        
        // 随机生成食物的位置
        float x = Random.Range(-boundary, boundary);
        float z = Random.Range(-boundary, boundary);
        
        // 确保食物不会生成在蛇身上
        bool validPosition = false;
        while (!validPosition)
        {
            validPosition = true;
            
            // 检查是否与蛇头重叠
            if (Vector3.Distance(new Vector3(x, 0, z), head.transform.localPosition) < 0.5f)
            {
                validPosition = false;
            }
            
            // 检查是否与蛇身重叠
            foreach (GameObject bodyPart in bodyParts)
            {
                if (Vector3.Distance(new Vector3(x, 0, z), bodyPart.transform.localPosition) < 0.5f)
                {
                    validPosition = false;
                    break;
                }
            }
            
            // 如果位置无效，重新生成
            if (!validPosition)
            {
                x = Random.Range(-boundary, boundary);
                z = Random.Range(-boundary, boundary);
            }
        }
        
        // 设置食物的位置
        food.transform.position = new Vector3(x, 0.5f, z);
    }
    
    // 收集食物
    void CollectFood()
    {
        // 增加得分
        score++;
        Debug.Log("Score: " + score);
        
        // 更新得分显示
        UpdateScoreDisplay();
        
        // 生成新的身体段
        GameObject newBodyPart = Instantiate(head, transform);
        newBodyPart.name = "Body" + (bodyParts.Count + 1);
        
        // 如果有身体段，将新身体段放在最后一个身体段的位置
        if (bodyParts.Count > 0)
        {
            newBodyPart.transform.localPosition = bodyParts[bodyParts.Count - 1].transform.localPosition;
        }
        else
        {
            // 如果没有身体段，将新身体段放在蛇头的位置
            newBodyPart.transform.localPosition = head.transform.localPosition;
        }
        
        // 为新身体段设置BodyMaterial材质
        Renderer renderer = newBodyPart.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material bodyMaterial = Resources.Load<Material>("Materials/BodyMaterial");
            if (bodyMaterial != null)
            {
                renderer.material = bodyMaterial;
            }
        }
        
        bodyParts.Add(newBodyPart);
        
        // 重新生成食物
        SpawnFood();
    }
    
    // 游戏结束
    void GameOver()
    {
        gameOver = true;
        Debug.Log("Game Over! Score: " + score);
        SetGameState(GameState.GameOver);
    }
    
    // 禁用和释放输入动作
    void OnDestroy()
    {
        if (moveUpAction != null)
        {
            moveUpAction.Disable();
            moveUpAction.Dispose();
        }
        if (moveDownAction != null)
        {
            moveDownAction.Disable();
            moveDownAction.Dispose();
        }
        if (moveLeftAction != null)
        {
            moveLeftAction.Disable();
            moveLeftAction.Dispose();
        }
        if (moveRightAction != null)
        {
            moveRightAction.Disable();
            moveRightAction.Dispose();
        }
    }
}