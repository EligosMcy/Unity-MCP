using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;

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
    // 身体管理器
    private SnakeBodyManager _bodyManager;

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

    // 移动计时器
    private float _moveTimer = 0.0f;

    // 移动间隔
    private float _moveInterval = 0.2f;

    // 移动方向
    private Vector3 _direction = Vector3.right;

    // 游戏是否结束
    private bool _gameOver = false;

    // 游戏结束标志（防止重复触发）
    private bool _isGameOverCalled = false;



    // 得分
    private int _score = 0;

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
            _boundary = _gameSetting.boundary;
            _moveInterval = _gameSetting.moveInterval;

            // 更新自动寻路组件的边界值
            if (_autoPathfinding != null)
            {
                _autoPathfinding.SetBoundary(_boundary);
            }
        }
    }

    void Start()
    {
        // 初始化身体管理器
        if (_bodyManager != null)
        {
            _bodyManager.Initialize(_gameSetting, _head);
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
        _foodManager.Initialize(_head, _bodyManager.GetBodyParts());
        _uiManager.Initialize();
        _uiManager.GetStartButton().onClick.AddListener(StartGame);

        // 初始化自动寻路组件
        if (_autoPathfinding != null)
        {
            _autoPathfinding.Initialize(_foodManager, this, _boundary);
            _autoPathfinding.UpdateBodyPartsReference(_bodyManager.GetBodyParts());
        }

        // 更新初始得分显示
        _uiManager.SetScore(_score);

        // 初始化身体动画事件
        InitializeBodyEvents();

        // 设置游戏状态为Start
        SetGameState(GameState.Start);
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
        // 重置蛇头位置
        _head.transform.localPosition = Vector3.zero;

        // 重置移动方向
        _direction = Vector3.right;

        // 重置身体
        if (_bodyManager != null)
        {
            _bodyManager.ResetBody();
        }

        // 更新食物管理器的身体段引用
        _foodManager.UpdateBodyPartsReference(_bodyManager.GetBodyParts());

        // 更新自动寻路组件的身体段引用
        if (_autoPathfinding != null)
        {
            _autoPathfinding.UpdateBodyPartsReference(_bodyManager.GetBodyParts());
        }

        // 清除食物
        _foodManager.ClearFood();
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
            if (_moveTimer >= _gameSetting.moveInterval)
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

        // 计算目标位置
        Vector3 targetPosition = _head.transform.localPosition + _direction;

        // 使用 DoTween 移动蛇头
        _head.transform.DOLocalMove(targetPosition, _moveInterval).SetEase(Ease.Linear);

        // 旋转蛇头以匹配移动方向
        RotateHead();

        // 移动蛇身
        if (_bodyManager != null)
        {
            _bodyManager.MoveBodyParts(targetPosition, previousPosition, _moveInterval);
        }

        // 检测位置碰撞（边界、身体、食物）
        CheckPositionCollisions(targetPosition);
    }

    // 检测位置碰撞
    void CheckPositionCollisions(Vector3 targetPosition)
    {
        // 检测边界碰撞
        if (Mathf.Abs(targetPosition.x) > _boundary || Mathf.Abs(targetPosition.z) > _boundary)
        {
            GameOver();
            return;
        }

        // 检测身体碰撞
        List<GameObject> bodyParts = _bodyManager.GetBodyParts();
        foreach (GameObject bodyPart in bodyParts)
        {
            if (bodyPart != null && Vector3.Distance(targetPosition, bodyPart.transform.localPosition) < 0.5f)
            {
                GameOver();
                return;
            }
        }

        // 检测食物碰撞
        CheckFoodCollision(targetPosition);
    }

    // 检测食物碰撞
    void CheckFoodCollision(Vector3 snakePosition)
    {
        // 检测小食物
        GameObject food = _foodManager.GetCurrentFood();
        if (food != null && Vector3.Distance(snakePosition, food.transform.localPosition) < 0.5f)
        {
            CollectFood();
            return;
        }

        // 检测大食物
        GameObject bigFood = _foodManager.GetCurrentBigFood();
        if (bigFood != null && Vector3.Distance(snakePosition, bigFood.transform.localPosition) < 0.5f)
        {
            CollectBigFood();
        }
    }

    // 旋转蛇头以匹配移动方向
    void RotateHead()
    {
        if (_head != null)
        {
            // 根据移动方向计算旋转角度
            float angle = 0f;

            if (_direction == Vector3.forward)
            {
                angle = 0f; // 向前 (Z轴正方向)
            }
            else if (_direction == Vector3.right)
            {
                angle = 90f; // 向右 (X轴正方向)
            }
            else if (_direction == Vector3.back)
            {
                angle = 180f; // 向后 (Z轴负方向)
            }
            else if (_direction == Vector3.left)
            {
                angle = 270f; // 向左 (X轴负方向)
            }

            // 应用旋转
            _head.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    // 收集食物
    void CollectFood()
    {
        // 增加得分
        _score++;

        // 更新得分显示
        _uiManager.SetScore(_score);

        // 启动身体变色动画
        if (_bodyManager != null)
        {
            _bodyManager.StartColorChangeAnimation(_gameSetting.targetColor);
        }

        // 销毁当前食物并重新生成
        _foodManager.ClearFood();
        _foodManager.SpawnFood();
    }

    // 收集大食物
    void CollectBigFood()
    {
        // 增加得分（大食物分数）
        int bigFoodScore = _foodManager.GetBigFoodScore();
        _score += bigFoodScore;

        // 更新得分显示
        _uiManager.SetScore(_score);

        // 启动身体变色动画（大食物使用特殊颜色）
        if (_bodyManager != null)
        {
            _bodyManager.StartColorChangeAnimation(_gameSetting.targetColor);
        }

        // 销毁当前大食物并重新生成
        _foodManager.ClearBigFood();
        _foodManager.SpawnFood();
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

    // 初始化身体动画事件
    private void InitializeBodyEvents()
    {
        if (_bodyManager != null)
        {
            _bodyManager.OnBodyAnimationComplete += OnBodyAnimationComplete;
        }
    }

    // 身体动画完成回调
    private void OnBodyAnimationComplete()
    {
        // 更新食物管理器的身体段引用
        _foodManager.UpdateBodyPartsReference(_bodyManager.GetBodyParts());

        // 更新自动寻路组件的身体段引用
        if (_autoPathfinding != null)
        {
            _autoPathfinding.UpdateBodyPartsReference(_bodyManager.GetBodyParts());
        }
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