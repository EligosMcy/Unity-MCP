using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    // 游戏状态枚举
    private enum GameState { Start, Playing, GameOver }

    // 当前游戏状态
    private GameState _currentState = GameState.Start;

    [SerializeField]
    private GameSettingScriptableObject _gameSetting;

    [SerializeField]
    // 蛇头的引用
    private GameObject _head;

    [SerializeField]
    // 蛇身的游戏对象列表
    private List<GameObject> _bodyParts = new List<GameObject>();

    [SerializeField]
    // 输入系统
    private InputActionProperty _moveInputActionProperty;

    [SerializeField]
    // 自动模式切换输入
    private InputActionProperty _autoModeInputActionProperty;

    [SerializeField]
    // 食物管理器
    private FoodManager _foodManager;

    [SerializeField]
    // 自动寻路组件
    private AutoPathfinding _autoPathfinding;

    [SerializeField]
    // UI管理器
    private UIManager _uiManager;

    // 游戏区域的边界
    private float _boundary = 4.5f;

    // 蛇身的预制体
    private GameObject _bodyPrefab;

    // 移动计时器
    private float _moveTimer = 0.0f;

    // 移动间隔
    private float _moveInterval = 0.2f;

    // 蛇的移动速度
    private float _moveSpeed = 1.0f;

    // 移动方向
    private Vector3 _direction = Vector3.right;

    // 游戏是否结束
    private bool _gameOver = false;

    // 游戏结束标志（防止重复触发）
    private bool _isGameOverCalled = false;

    // 得分
    private int _score = 0;

    // 身体大小缩放系数
    private float _bodyScaleFactor = 0.8f;

    // 自动模式标志
    private bool _autoMode = false;

    // 自动模式统计信息
    private int _autoModeRounds = 0;
    private List<int> _autoModeScores = new List<int>();
    private int _autoModeHighScore = 0;

    private void Awake()
    {
        if (_gameSetting == null)
        {
            _gameSetting = Resources.Load<GameSettingScriptableObject>("GameSettings/DefaultGameSetting");
        }

        if (_gameSetting != null)
        {
            _bodyPrefab = _gameSetting.bodyPrefab;
            _boundary = _gameSetting.boundary;
            _moveSpeed = _gameSetting.moveSpeed;
            _moveInterval = _gameSetting.moveInterval;
            _bodyScaleFactor = _gameSetting.bodyScaleFactor;

            // 更新自动寻路组件的边界值
            if (_autoPathfinding != null)
            {
                _autoPathfinding.SetBoundary(_boundary);
            }
        }
    }

    void Start()
    {
        // 初始化蛇身列表，添加初始的身体段
        foreach (Transform child in transform)
        {
            if (child.name != "Head")
            {
                _bodyParts.Add(child.gameObject);
            }
        }

        // 启用输入动作
        _moveInputActionProperty.action.Enable();

        // 启用自动模式输入动作
        if (_autoModeInputActionProperty.action != null)
        {
            _autoModeInputActionProperty.action.Enable();
            _autoModeInputActionProperty.action.performed += ctx => ToggleAutoMode();
        }

        // 初始化管理器
        _foodManager.Initialize(_head, _bodyParts);
        _uiManager.Initialize();
        _uiManager.GetStartButton().onClick.AddListener(StartGame);

        // 初始化自动寻路组件
        if (_autoPathfinding != null)
        {
            _autoPathfinding.Initialize(_foodManager, this, _boundary);
            _autoPathfinding.UpdateBodyPartsReference(_bodyParts);
        }

        // 更新初始得分显示
        _uiManager.SetScore(_score);

        // 设置游戏状态为Start
        SetGameState(GameState.Start);

        // 初始调整身体大小
        AdjustBodySizes();
    }

    // 设置游戏状态
    void SetGameState(GameState state)
    {
        _currentState = state;

        switch (state)
        {
            case GameState.Start:
                // 显示开始按钮
                _uiManager.ShowStartButton(true);
                // 隐藏游戏结束文本
                _uiManager.ShowGameOverText(false, 0);
                // 暂停游戏
                Time.timeScale = 0;
                break;

            case GameState.Playing:
                // 隐藏开始按钮
                _uiManager.ShowStartButton(false);
                // 隐藏游戏结束文本
                _uiManager.ShowGameOverText(false, 0);
                // 开始游戏
                Time.timeScale = 1;
                // 生成初始食物
                _foodManager.SpawnFood();
                break;

            case GameState.GameOver:
                // 显示开始按钮
                _uiManager.ShowStartButton(true);
                // 显示游戏结束文本
                _uiManager.ShowGameOverText(true, _score);
                // 暂停游戏
                Time.timeScale = 0;
                break;
        }
    }

    // 开始游戏
    public void StartGame()
    {
        // 重置游戏结束标志
        _isGameOverCalled = false;
        
        // 取消所有挂起的 Invoke 调用
        CancelInvoke("StartGame");
        
        // 重置得分
        _score = 0;
        _uiManager.SetScore(_score);

        // 重置蛇的位置和长度
        ResetSnake();

        // 设置游戏状态为Playing
        SetGameState(GameState.Playing);
    }

    // 重置蛇
    void ResetSnake()
    {
        // 清除所有身体段
        foreach (GameObject bodyPart in _bodyParts)
        {
            Destroy(bodyPart);
        }
        _bodyParts.Clear();

        // 重置蛇头位置
        _head.transform.localPosition = Vector3.zero;

        // 重置移动方向
        _direction = Vector3.right;

        // 生成初始身体段
        GameObject body1;
        if (_bodyPrefab != null)
        {
            body1 = Instantiate(_bodyPrefab, transform);
        }
        else
        {
            // 如果预制体未设置，使用默认从head实例化
            body1 = Instantiate(_head, transform);

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
        }
        body1.name = "Body1";
        body1.transform.localPosition = new Vector3(-1, 0, 0);

        _bodyParts.Add(body1);

        // 更新食物管理器的身体段引用
        _foodManager.UpdateBodyPartsReference(_bodyParts);

        // 更新自动寻路组件的身体段引用
        if (_autoPathfinding != null)
        {
            _autoPathfinding.UpdateBodyPartsReference(_bodyParts);
        }

        // 清除食物
        _foodManager.ClearFood();

        // 调整身体大小
        AdjustBodySizes();
    }

    void Update()
    {
        // 只有在游戏状态为Playing时才处理输入和移动
        if (_currentState == GameState.Playing && !_isGameOverCalled)
        {
            // 自动模式下执行寻路
            if (_autoMode)
            {
                AutoPathfinding();
            }
            else
            {
                // 手动模式下处理输入
                HandleInput();
            }

            // 更新移动
            _moveTimer += Time.deltaTime;
            if (_moveTimer >= _moveInterval)
            {
                Move();
                _moveTimer = 0.0f;
            }
        }
    }

    // 处理玩家输入
    void HandleInput()
    {
        if (_moveInputActionProperty.action != null)
        {
            Vector2 inputValue = _moveInputActionProperty.action.ReadValue<Vector2>();

            // 根据输入值更新移动方向
            if (Mathf.Abs(inputValue.y) > Mathf.Abs(inputValue.x))
            {
                // 垂直输入优先
                if (inputValue.y > 0 && _direction != Vector3.back)
                {
                    _direction = Vector3.forward;
                }
                else if (inputValue.y < 0 && _direction != Vector3.forward)
                {
                    _direction = Vector3.back;
                }
            }
            else if (Mathf.Abs(inputValue.x) > 0)
            {
                // 水平输入
                if (inputValue.x < 0 && _direction != Vector3.right)
                {
                    _direction = Vector3.left;
                }
                else if (inputValue.x > 0 && _direction != Vector3.left)
                {
                    _direction = Vector3.right;
                }
            }
        }
    }

    // 移动蛇
    void Move()
    {
        // 如果游戏已经结束，不执行移动
        if (_isGameOverCalled)
            return;
        
        // 记录蛇头的当前位置
        Vector3 previousPosition = _head.transform.localPosition;

        // 移动蛇头
        _head.transform.localPosition += _direction;

        // 检查自身碰撞
        foreach (GameObject bodyPart in _bodyParts)
        {
            if (Vector3.Distance(_head.transform.localPosition, bodyPart.transform.localPosition) < 0.5f)
            {
                GameOver();
                return;
            }
        }

        // 移动蛇身
        if (_bodyParts.Count > 0)
        {
            for (int i = 0; i < _bodyParts.Count; i++)
            {
                Vector3 tempPosition = _bodyParts[i].transform.localPosition;
                _bodyParts[i].transform.localPosition = previousPosition;
                previousPosition = tempPosition;
            }
        }

        // 调整身体大小
        AdjustBodySizes();
    }

    // 碰撞检测 - 由SnakeHeadTrigger调用
    public void HandleTriggerEnter(Collider other)
    {
        // 如果游戏已经结束，不处理碰撞
        if (_isGameOverCalled)
            return;
        
        // 检查食物碰撞
        if (other.gameObject.CompareTag(_gameSetting.foodTag))
        {
            CollectFood();
        }

        // 检查边界碰撞
        if (other.gameObject.CompareTag(_gameSetting.boundaryTag))
        {
            GameOver();
        }
    }

    // 收集食物
    void CollectFood()
    {
        // 增加得分
        _score++;

        // 更新得分显示
        _uiManager.SetScore(_score);

        // 生成新的身体段
        GameObject newBodyPart;
        if (_bodyPrefab != null)
        {
            newBodyPart = Instantiate(_bodyPrefab, transform);
        }
        else
        {
            // 如果预制体未设置，使用默认从head实例化
            newBodyPart = Instantiate(_head, transform);

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
        }
        newBodyPart.name = "Body" + (_bodyParts.Count + 1);

        // 如果有身体段，将新身体段放在最后一个身体段的位置
        if (_bodyParts.Count > 0)
        {
            newBodyPart.transform.localPosition = _bodyParts[_bodyParts.Count - 1].transform.localPosition;
        }
        else
        {
            // 如果没有身体段，将新身体段放在蛇头的位置
            newBodyPart.transform.localPosition = _head.transform.localPosition;
        }

        _bodyParts.Add(newBodyPart);

        // 更新食物管理器的身体段引用
        _foodManager.UpdateBodyPartsReference(_bodyParts);

        // 更新自动寻路组件的身体段引用
        if (_autoPathfinding != null)
        {
            _autoPathfinding.UpdateBodyPartsReference(_bodyParts);
        }

        // 销毁当前食物并重新生成
        _foodManager.ClearFood();
        _foodManager.SpawnFood();

        // 调整身体大小
        AdjustBodySizes();
    }

    // 调整身体大小
    void AdjustBodySizes()
    {
        int bodyCount = _bodyParts.Count;
        if (bodyCount == 0)
            return;

        // 计算需要逐渐变小的身体段数量（最多5个）
        int shrinkCount = Mathf.Min(5, bodyCount);

        // 从头部开始往后逐渐变小
        for (int i = 0; i < bodyCount; i++)
        {
            GameObject bodyPart = _bodyParts[i];
            if (bodyPart != null)
            {
                if (i < shrinkCount)
                {
                    // 逐渐变小
                    float scaleFactor;
                    if (shrinkCount > 1)
                    {
                        scaleFactor = 1.0f - (i * (1.0f - _bodyScaleFactor) / (shrinkCount - 1));
                    }
                    else
                    {
                        // 避免除以零，当只有一个身体段时使用默认大小
                        scaleFactor = 1.0f;
                    }
                    bodyPart.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                }
                else
                {
                    // 最小大小
                    bodyPart.transform.localScale = new Vector3(_bodyScaleFactor, _bodyScaleFactor, _bodyScaleFactor);
                }
            }
        }
    }

    // 游戏结束
    void GameOver()
    {
        // 防止重复调用 GameOver
        if (_isGameOverCalled)
            return;
        
        _isGameOverCalled = true;
        _gameOver = true;
        Debug.Log("Game Over! Score: " + _score);
        
        // 如果是自动模式，记录统计信息并自动重新开始
        if (_autoMode)
        {
            // 记录分数
            _autoModeScores.Add(_score);
            _autoModeRounds++;
            
            // 更新最高分
            if (_score > _autoModeHighScore)
            {
                _autoModeHighScore = _score;
            }
            
            // 输出统计信息
            Debug.Log("Auto Mode Round: " + _autoModeRounds);
            Debug.Log("Current Score: " + _score);
            Debug.Log("High Score: " + _autoModeHighScore);
            Debug.Log("All Scores: " + string.Join(", ", _autoModeScores));
            
            // 延迟一段时间后自动重新开始
            Invoke("StartGame", 1.0f);
            return;
        }
        
        SetGameState(GameState.GameOver);
    }

    // 切换自动模式
    public void ToggleAutoMode()
    {
        _autoMode = !_autoMode;
        _uiManager.UpdateAutoModeDisplay(_autoMode);
        Debug.Log("Auto mode " + (_autoMode ? "enabled" : "disabled"));
        
        // 如果启用自动模式，重置统计信息
        if (_autoMode)
        {
            _autoModeRounds = 0;
            _autoModeScores.Clear();
            _autoModeHighScore = 0;
            Debug.Log("Auto mode statistics reset.");
        }
    }

    // 自动寻路到食物
    private void AutoPathfinding()
    {
        // 使用自动寻路组件计算路径
        if (_autoPathfinding != null)
        {
            Vector3 headPos = _head.transform.localPosition;
            _direction = _autoPathfinding.CalculatePath(headPos);
        }
    }



    // 禁用和释放输入动作
    void OnDestroy()
    {
        if (_moveInputActionProperty.action != null)
        {
            _moveInputActionProperty.action.Disable();
            _moveInputActionProperty.action.Dispose();
        }

        if (_autoModeInputActionProperty.action != null)
        {
            _autoModeInputActionProperty.action.Disable();
            _autoModeInputActionProperty.action.Dispose();
        }
    }
}