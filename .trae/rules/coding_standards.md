# Unity C# 编码规范

## 命名约定

### 类名
- 使用 PascalCase
- 示例: `PlayerController`, `GameManager`

### 方法名
- **公共方法**: 使用 PascalCase
  - 示例: `GetPlayerData()`, `SetScore()`
- **私有方法**: 使用 camelCase（小写字母开头）
  - 示例: `updatePosition()`, `checkObstacles()`
- **Unity生命周期方法**: 遵循Unity官方命名规范（PascalCase）
  - 示例: `Awake()`, `Start()`, `Update()`, `OnCollisionEnter()`, `OnEnable()`, `OnDisable()`
  - **注意**: Unity生命周期方法不受私有方法命名规则限制，必须使用Unity官方定义的名称

### 变量名
- **公共变量**: 使用 PascalCase
  - 示例: `Score`, `Health`, `MoveSpeed`
- **私有变量**: 使用下划线前缀 + camelCase
  - 示例: `_playerName`, `_privateVariable`, `_currentHealth`
- **局部变量**: 使用 camelCase
  - 示例: `playerScore`, `newPosition`

### 常量
- 使用 PascalCase
- 示例: `MaxHealth`, `JumpForce`

### 事件
- 使用 PascalCase，通常以 "On" 开头
- 示例: `OnPlayerReady`, `OnScoreChanged`, `OnPositionChanged`

## 代码风格

### 缩进
- 统一使用4个空格缩进

### 大括号
- 所有语句块（包括单行语句）都使用大括号
```csharp
// 正确
if (Score > 0)
{
    Debug.Log("Score is positive");
}

// 错误
if (Score > 0)
    Debug.Log("Score is positive");
```

### 注释
- 使用中文注释
- 保持简洁清晰

### 特性使用
- 合理使用Unity特性，如 `[SerializeField]`, `[Header]`, `[Tooltip]`, `[Range]`

## 最佳实践

### 避免魔法数字
- 使用常量代替硬编码的数字
```csharp
const float jumpForce = 10.0f;
```

### 单一职责原则
- 每个方法只做一件事
- 拆分复杂方法为多个小方法

### 空值检查
- 在访问可能为null的对象前进行检查
```csharp
if (collision != null && collision.gameObject.CompareTag("Obstacle"))
{
    // 处理逻辑
}
```

### 使用属性访问器
- 优先使用属性而不是公共字段
```csharp
public int Score { get; private set; }
```

### 输入系统
- 只使用新版的InputSystem，不要使用旧版的InputManager
- 示例：使用 `InputSystem` 命名空间和相关API进行输入处理
