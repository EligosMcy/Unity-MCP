# Demo 3-1：角色情绪即时切换与行为反馈（Demo Mode A）实施计划

本 Demo 聚焦“情绪即状态”的即时可见性：在同一核心角色的基础上，通过一键切换四类情绪（疲惫/精力充沛/烦躁/兴奋），在 200ms 内同时驱动文本风格、语气/用词、Emoji 密度、姿态/表情、动画速度、表达强度与注意力策略共 7 个维度发生显著变化。玩家可在自由对话中直接感受“情绪 + 基础性格”的个性化回复，同时角色呈现与 UI 动效共同放大差异，确保观众几秒内“看得见、听得到、感受得到”。Demo 提供 5 分钟情绪记忆与重置能力，既支持快速对比情绪差异，也可连续体验某种情绪下的持续互动，旨在呈现有品味、有反应、有关怀的“活的角色”表达。

## 1. 目标与范围
- 核心目标：验证“切换情绪即换角色状态”，让观众在数秒内清晰感知差异。
- 展示范围：单一核心角色（基础性格：活泼可爱的小玩伴），中立基线 + 4 种情绪（疲惫 / 精力充沛 / 烦躁 / 兴奋）。
- 多维反馈：说话语气、用词、Emoji 密度、肢体/形象、动画速度、表达强度、注意力，共 7 个维度同步变化。
- 交互方式：一键切换情绪 + 自由对话（文本/可选语音），支持情绪短期记忆（5 分钟），一键重置中立。

## 2. 成功标准（验收条件）
- 切换四种情绪时，7 个维度在 200ms 内同步生效，无可见延迟。
- 对话文本风格差异显著，3 秒内可感知角色“活的”表达（非模板机器人）。
- Emoji、句式长度、语气词/感叹词、注意力行为（延伸/打断/冷淡）随情绪明显变化。
- 角色肢体与动画速度一致匹配情绪（极慢/适中/极慢僵硬/极快），UI 中有明确情绪指示。
- 情绪记忆有效：选“兴奋”后 5 分钟内保持兴奋风格，重置返回中立。

## 3. 关键流程
1) 选择角色 → 进入自由对话（中立）  
2) 点击情绪按钮（如兴奋） → 7 维度即时变化  
3) 玩家输入或语音对话 → 基于“当前情绪 + 基础性格”生成回复  
4) 切换其他情绪对比 → 可随时“重置情绪”

## 4. 系统架构总览
- 角色预设模块：角色基础形象与中立状态。
- 情绪系统模块：集中管理当前情绪、定时记忆、广播变化事件。
- UI 模块：四个情绪按钮 + 重置按钮 + 情绪指示；聊天输入/发送/语音按钮。
- 对话模块：规则/模板驱动的文本生成器（后续可插拔大模型）；可选 TTS。
- 表达呈现模块：驱动动画参数、姿态、特效、UI 动效；统一由“情绪配置”映射。
- 同步器（Presenter）：接收情绪变化，协调文本、动画、特效等子系统同时响应。

## 5. 数据模型与参数映射（Unity 友好）
- EmotionType（枚举）：Neutral / Tired / Energetic / Irritable / Excited
- EmotionProfile（配置）：用于 7 维度参数化，示例字段
  - VoiceRate（语速 0.5~2.0）、VoiceVolume（0~1）  
  - TextLengthBias（短/中/长）、AffirmationBias（积极/中立/消极）  
  - EmojiDensity（0~1）、EmojiPool（表情集合）  
  - PoseId/LayerWeight（动画姿态/层权重）、AnimSpeed（0.5~2.0）  
  - ExpressIntensity（0~1）、AttentionMode（Focused/Random/Reject）
- 同步策略：单一“SetEmotion(EmotionType e)”入口，内部一次性下发所有子系统更新，保证一致性与无抖动。

### 5.1 枚举与取值说明
- EmotionType
  - Neutral（中立）
  - Tired（疲惫）
  - Energetic（精力充沛）
  - Irritable（烦躁）
  - Excited（兴奋）
- TextLengthBias（文本长度偏好）
  - Short（短句为主，节奏快）
  - Medium（中等长度，均衡）
  - Long（长句为主，更丰富修饰）
- AffirmationBias（语义倾向）
  - Positive（积极、褒义、鼓励）
  - Neutral（中性、客观）
  - Negative（消极、克制、冷淡）
- AttentionMode（注意力策略）
  - Focused（高专注，紧贴话题、精准回应）
  - Random（易分心，10~20% 概率延伸/跑题）
  - Reject（拒聊/冷淡，倾向短句结束）
- AnimatorPoseId（动画姿态编号，对应 Animator Int 参数）
  - Neutral = 0
  - Tired = 1
  - Energetic = 2
  - Irritable = 3
  - Excited = 4
- EmojiCategory（可选，用于组织 EmojiPool）
  - Positive（如 😊、✨、🙂）
  - Celebration（如 🎉、🥳、🚀）
  - Neutral（如 🙂、😉）
  - Negative（如 😒、🙄、😑）
 - PunctuationStyle（标点风格）
   - Minimal（极简，少标点）
   - Neutral（中性）
   - Exclamatory（感叹偏好，！较多）
   - EllipsisHeavy（省略号偏好，…较多）
 - VisualMoodPreset（环境情绪预设）
   - Neutral（中性）
   - LowSaturationVignette（低饱和 + 深暗角）
   - HighContrastSparkle（高对比 + 微粒子）

### 5.2 EmotionProfile 字段定义（类型/范围/用途）
- VoiceRate：float，0.5~2.0（语速，影响 TTS 或语气描述与文字节奏）
- VoiceVolume：float，0~1（音量，影响描述或音频渲染）
- TextLengthBias：enum（Short/Medium/Long，影响句式长度模板选择）
- AffirmationBias：enum（Positive/Neutral/Negative，影响措辞褒贬）
- EmojiDensity：float，0~1（插入 Emoji 的概率或密度）
- EmojiPool：string[]（候选 Emoji 集；可按 EmojiCategory 组织）
- PoseId：int（AnimatorPoseId 枚举对应的值）
- LayerWeight：float 或 float[]（情绪层的权重，0~1）
- AnimSpeed：float，0.5~2.0（动画速度缩放）
- ExpressIntensity：float，0~1（表达强度，用于夸张程度/表情幅度等）
- AttentionMode：enum（Focused/Random/Reject，决定回复策略）
 - TypingSpeed：float，字符/秒（打字机速度，用于气泡逐字显示）
 - ResponseDelay：float，秒（开始回复前的延迟）
 - PunctuationStyle：enum（Minimal/Neutral/Exclamatory/EllipsisHeavy）
 - IdleRandomActions：ActionItem[]（小动作触发器列表；见下）
 - VisualPreset：enum（VisualMoodPreset，环境情绪预设）
 - VolumeBlendWeight：float，0~1（全局 Volume 混合权重，按情绪设定）
 - ActionItem 结构建议：
   - Clip/VFX/SFX 引用、权重（0~1）、冷却（秒）、最小间隔（秒）
   - 触发条件：Idle 时长阈值、随机窗口（如每 5~15 秒间）

校验与编辑器约束（建议在 OnValidate 中钳制）：
- VoiceRate 在 [0.5, 2.0]；VoiceVolume、EmojiDensity、ExpressIntensity 在 [0, 1]。
- PoseId 必须匹配 Animator Controller 的参数映射；LayerWeight 在 [0, 1]。
- EmojiPool 至少包含 1 个 Emoji；空数组时回退到默认池。

### 5.3 数据加载与运行期映射
- 配置来源：ScriptableObject（Resources/DemoModeA/EmotionProfiles.asset 或子资产）；运行前可在 Inspector 快速调参。
- 应用顺序：EmotionController.SetEmotion → 读取对应 EmotionProfile → CharacterPresenter/DialogueController 同步消费。
- 与 Animator 的绑定：
  - Int 参数 PoseId ← AnimatorPoseId（0~4）
  - Float 参数 AnimSpeed ← EmotionProfile.AnimSpeed
  - Float 参数 Layer 权重 ← EmotionProfile.LayerWeight（如有）
  - 缺失绑定时执行安全降级（仅调速/不切姿态）。

### 5.4 推荐默认值（与第 6 节基线保持一致）
- Neutral（中立）：VoiceRate 1.0、VoiceVolume 0.7、TextLengthBias Medium、AffirmationBias Neutral、EmojiDensity 0.1、AnimSpeed 1.0、ExpressIntensity 0.5、AttentionMode Focused、PoseId=0
- Tired（疲惫）：VoiceRate 0.7、VoiceVolume 0.5、TextLengthBias Short、AffirmationBias Neutral、EmojiDensity 0.05、AnimSpeed 0.6、ExpressIntensity 0.3、AttentionMode Random、PoseId=1
- Energetic（精力充沛）：VoiceRate 1.2、VoiceVolume 0.8、TextLengthBias Medium、AffirmationBias Positive、EmojiDensity 0.3、AnimSpeed 1.0、ExpressIntensity 0.7、AttentionMode Focused、PoseId=2
- Irritable（烦躁）：VoiceRate 0.9、VoiceVolume 0.7、TextLengthBias Short、AffirmationBias Negative、EmojiDensity 0.05、AnimSpeed 0.6（僵硬）、ExpressIntensity 0.3、AttentionMode Reject、PoseId=3
- Excited（兴奋）：VoiceRate 1.4、VoiceVolume 0.9、TextLengthBias Long、AffirmationBias Positive、EmojiDensity 0.8、AnimSpeed 1.6、ExpressIntensity 0.9、AttentionMode Focused、PoseId=4

### 5.5 非线性反馈与环境预设映射（示例）
- 疲惫（Tired）
  - IdleRandomActions：打哈欠、垂头（低频触发，冷却 8~15s）
  - TypingSpeed：慢（如 6~8 chars/s），ResponseDelay：~1.5s
  - PunctuationStyle：EllipsisHeavy（…为主，句首可不大写/少标点）
  - VisualPreset：LowSaturationVignette，VolumeBlendWeight：0.6~0.8
- 精力充沛（Energetetic）
  - IdleRandomActions：轻点头/轻挥手（中低频）
  - TypingSpeed：中快（如 12~14 chars/s），ResponseDelay：~0.4s
  - PunctuationStyle：Neutral/轻微 Exclamatory
  - VisualPreset：HighContrastSparkle（弱粒子），VolumeBlendWeight：0.3~0.5
- 烦躁（Irritable）
  - IdleRandomActions：啧舌（SFX）、视线移开（中频）
  - TypingSpeed：中（如 10~12 chars/s），ResponseDelay：~0.8s
  - PunctuationStyle：Minimal（短促、少修饰）
  - VisualPreset：LowSaturationVignette，VolumeBlendWeight：0.5~0.7
- 兴奋（Excited）
  - IdleRandomActions：原地小跳、星星眼（VFX）（中高频，冷却 5~10s）
  - TypingSpeed：很快（如 16~20 chars/s），ResponseDelay：~0.1s
  - PunctuationStyle：Exclamatory（“！”密集）
  - VisualPreset：HighContrastSparkle（弱粒子），VolumeBlendWeight：0.5~0.7

## 6. 4 种情绪的 7 维度映射（实施基线）
| 维度 | 疲惫 | 精力充沛 | 烦躁 | 兴奋 |
| --- | --- | --- | --- | --- |
| 说话语气 | 语速0.7、音量0.5、拖尾 | 语速1.2、音量0.8、明快 | 语速0.9、音量0.7、短促 | 语速1.4、音量0.9、高昂 |
| 用词选择 | 短句+语气词“啊/哦” | 生动、比喻、褒义词 | 生硬、命令式、少修饰 | 夸张、感叹词多“！” |
| Emoji 密度 | 0.05（几乎无） | 0.3（😊✨） | 0.05（😒🙄） | 0.8（🎉🥳🚀） |
| 肢体/形象 | 垂头、慢动作 | 挺胸、抬头、轻快 | 皱眉、叉腰、僵硬 | 蹦跳、挥手、眼亮 |
| 动画速度 | 0.6 | 1.0 | 0.6（僵硬） | 1.6 |
| 表达强度 | 0.3（敷衍） | 0.7（积极） | 0.3（冷淡） | 0.9（夸张） |
| 注意力 | Random（偶尔跑题） | Focused（精准回应） | Reject（拒聊倾向） | Focused（主动延伸） |

> 注：以上为基线参数，运行期可通过 Inspector/ScriptableObject 微调。

## 7. UI/交互设计
- 顶部情绪栏：4 个按钮（疲惫/精力充沛/烦躁/兴奋），图标化，点击即触发；右侧“重置为中立”。
- 中部角色区：2D 立绘或 3D 角色 + Animator；表情/姿态/速度随情绪变化。
- 底部聊天区：输入框 + 发送按钮 + 语音（长按录音/可后续接入）；对话气泡使用 TextMeshPro。
- 动效与反馈：按钮点击有轻微缩放/高亮（DOTween），情绪切换有过渡色/表情小动画。
- 打字机效果：逐字显示速度受 TypingSpeed 控制，支持快进；显示期间有“输入中”指示。
- 重置反馈：Reset 时播放“深呼吸/闪光”特效（VFX），并回落至中立视觉预设。
- 环境联动：全局 Volume（暗角/色彩）与轻微粒子随情绪切换；UI 主题色保持一致映射。

## 8. 技术实现（Unity）
- 现有依赖复用：TextMeshPro、DOTween、新版 Input System（仓库已集成）。
- 动画方案：
  - Animator Controller：基础待机/呼吸循环；4 种情绪可做独立 State 或统一状态 + 参数驱动（AnimSpeed、LayerWeight、BlendShape/表情）。
  - 2D 方案：切换不同表情 Sprite/Animator Layer；3D 方案：表情 BlendShape + 姿态动画片段。
- 对话生成（首版规则引擎）：
  - 根据 EmotionProfile 选择句式模板（短/长、积极/消极）、插入表情符、控制标点与语气词。
  - 注意力策略：Focused 精准回答要点；Random 有 10~20% 概率延伸或跑题；Reject 直接拒绝/冷淡短句。
  - 后续可替换为大模型接口（保持模块边界与接口稳定）。
- 情绪记忆：
  - 结构：CurrentEmotion + ExpireAt（UTC 时间）；切换时刷新 ExpireAt=Now+5min。
  - Update/Coroutine 监测过期自动回落中立；重置按钮立即清空并回中立。
- 性能与稳定：
  - 所有切换在一帧内下发；避免多处各自查找状态（统一由 Presenter 协调）。
  - 动画过渡：优先使用 Animator.CrossFadeInFixedTime(0.2f) 做姿态切换，骨骼插值更自然；同时调节 AnimSpeed。
  - DOTween：过渡序列可中断，新的情绪切换时 Kill 前序列并平滑接管，避免“瞬移”。
  - TextMeshPro：启用/绑定 TMP Sprite Asset，预生成常用 Emoji，避免丢贴图（紫块）。
  - Volume：全局 URP Volume 使用 Profile 覆盖或权重混合；CharacterPresenter 控制混合权重与轻量粒子开关。
  - 打字机：逐字显示采用帧驱动，不阻塞 UI；高速状态下提供跳字/快进。

## 9. 场景与脚本清单（命名遵循仓库规范）
- 场景：Scenes/DemoModeA.unity
- 预制体：Prefabs/Character_Demo.prefab（2D 或 3D，包含 Animator）
- 脚本（Assets/Scripts/EligosNewGame/ 或新建 DemoModeA/）：
  - EmotionType.cs（枚举）
  - EmotionProfile.cs（ScriptableObject 或纯数据类）
  - EmotionController.cs（管理当前情绪、广播事件、记忆）
  - CharacterPresenter.cs（监听情绪变化，驱动 Animator/表情/特效/UI）
  - DialogueController.cs（基于情绪生成回复）
  - EmotionUIButton.cs（UI 绑定：点击 → SetEmotion）
  - ChatUIController.cs（输入框/发送/展示对话气泡）
  - VoiceInputProvider.cs（可选：语音接入的占位实现）

### 9.1 脚本功能与需求清单
- EmotionType.cs
  - 定义：Neutral / Tired / Energetic / Irritable / Excited（保留中立）。
  - 要求：枚举值稳定，不随版本变动；提供 ToString/本地化映射的辅助方法（可在扩展类中）。
- EmotionProfile.cs
  - 类型：ScriptableObject（推荐）或纯数据类。每种情绪一份配置或集合映射。
  - 字段：VoiceRate、VoiceVolume、TextLengthBias、AffirmationBias、EmojiDensity、EmojiPool、PoseId/LayerWeight、AnimSpeed、ExpressIntensity、AttentionMode、TypingSpeed、ResponseDelay、PunctuationStyle、IdleRandomActions、VisualPreset、VolumeBlendWeight。
  - 编辑器：在 OnValidate 中做区间钳制（如 0~1、0.5~2.0），EmojiPool 非空校验。
  - 加载：提供静态获取接口或 Resources 加载；避免运行时 GC 峰值。
- EmotionController.cs
  - 状态：CurrentEmotion、ExpireAt、IsExpired（只读）；初始为 Neutral。
  - API：SetEmotion(EmotionType)、ResetEmotion()、GetRemainingTime(); 读写线程安全（主线程使用）。
  - 事件：OnEmotionChanged(EmotionType old, EmotionType @new)；切换时仅触发一次。
  - 记忆：SetEmotion 时刷新 ExpireAt=Now+5min；Update/Coroutine 超时自动回落 Neutral。
  - 防抖：连续点击同一情绪不重复广播；短间隔快速切换（<100ms）合并为一次变更。
- CharacterPresenter.cs
  - 依赖：Animator（必需）、可选 SpriteRenderer/SkinnedMeshRenderer、可选 VFX/ParticleSystem。
  - 参数映射：AnimSpeed(float)、PoseId(int) 或 LayerWeight(float[])；表情/BlendShape（可选）；VolumeBlendWeight（全局环境）。
  - 同步：接收 OnEmotionChanged，一帧内更新动画速度、姿态、表情与 Volume；使用 CrossFadeInFixedTime + DOTween 做 0.15~0.25s 平滑过渡。
  - 小动作：根据 IdleRandomActions 定时/随机触发 Clip/VFX/SFX（含冷却与权重）；可打断并安全恢复 Idle。
  - 兜底：缺失动画片段时执行安全降级（仅调速）；在 Inspector 明确暴露引用并校验。
- DialogueController.cs
  - 输入：用户消息（string）+ 当前 EmotionProfile。
  - 输出：角色回复（string），包含句式、语气词、Emoji 注入（按 EmojiDensity 随机插入），标点风格（PunctuationStyle）。
  - 打字机：逐字输出速度由 TypingSpeed 控制；首字延迟使用 ResponseDelay；提供取消/快进接口。
  - 策略：根据 AttentionMode 选择 Focused/Random/Reject 分支；Random 10~20% 跑题；Reject 返回冷淡短句。
  - 扩展：保留 async GenerateReplyAsync 入口以便后续接入 LLM/TTS；超时与空输入处理。
- EmotionUIButton.cs
  - 绑定：Unity UI Button；序列化 EmotionType；点击时调用 EmotionController.SetEmotion。
  - 反馈：点击缩放/高亮（DOTween）；当前选中情绪按钮高亮常驻；禁用状态时不可交互。
  - 稳定：短时间内多次点击合并；再次点击新情绪时优雅中断前一过渡（通过 Presenter）。
- ChatUIController.cs
  - 组件：TMP_InputField、Send Button、ScrollView/Content 容器、消息气泡预制体池。
  - 行为：Enter 发送、空白不发、最大长度限制（如 256）；发送后清空并聚焦输入框；显示“输入中”指示。
  - 展示：区分“玩家/角色”两类气泡样式；自动滚动到底部；支持 TMP 富文本 Emoji；逐字显隐与快进。
  - 性能：对象池复用气泡；大数据时裁剪历史（如只保留最近 N 条）。
- VoiceInputProvider.cs（可选）
  - 接口：StartRecording/StopRecording、OnTranscript(string text) 事件。
  - 状态：录音中指示；权限与设备不可用时的提示。
  - 替换：留出第三方/平台语音 SDK 的接入点，不阻塞文本流程。

## 10. 资产需求（最小可行）
- 角色形象：任选其一（2D/3D 均可）
  - 2D 方案（详细）
    - 立绘/贴图：中立 + 疲惫/精力充沛/烦躁/兴奋 共 5 张（或 1 张主体 + 表情层）。
    - 动画：基础待机/呼吸循环；表情切换图集或 Animator Layer；可选小幅摇摆/跳跃片段。
    - 资源格式：Sprite/Atlas（建议 2048 内），透明 PNG；Animator Controller（Demo 封装）。
  - 3D 方案（详细）
    - 模型：低/中模 1 个；绑定与骨骼规范；材质与法线贴图可选。
    - 动画：待机循环 + 4 个情绪姿态短片段（Idle_Tired/Energetic/Irritable/Excited）；可用 Mixamo/开源库。
    - 表情：BlendShape 若可用；无则用头部/手势姿态替代；Animator Controller 驱动参数 AnimSpeed/PoseId。
- UI（详细）
  - 情绪按钮：4 个图标（疲惫/精力充沛/烦躁/兴奋）+ 重置图标；选中与未选中两套状态。
  - 聊天气泡：玩家气泡与角色气泡各 1 套；圆角、阴影、背景色区分；支持 TMP 富文本。
  - 字体与皮肤：TMP 字体资源 1 套；UI 主题色（建议：中立灰、疲惫蓝灰、精力充沛绿、烦躁橙、兴奋粉/紫）。
  - 动效：按钮点击/切换高亮、情绪切换过渡（DOTween）；简单音效反馈（可选）。
- 音频（详细，可选）
  - 按钮点击/切换音：短促、不打扰；兴奋/疲惫可配不同音色提示。
  - 未来扩展：TTS 输出音色/语速与情绪同步；当前版本不强制。
- Animator 参数约定（便于统一对接）
  - Float：AnimSpeed（范围 0.5~2.0）
  - Int：PoseId（0=Neutral, 1=Tired, 2=Energetic, 3=Irritable, 4=Excited）
  - Float（可选）：LayerWeightEmotion（0~1）
- 目录结构建议（便于团队协作与复用）
  - Assets/Scenes/DemoModeA.unity
  - Assets/Scripts/DemoModeA/（或 EligosNewGame 子目录）
  - Assets/Prefabs/DemoModeA/Character_Demo.prefab
  - Assets/UI/DemoModeA/（Sprites、Buttons、ChatBubbles、TMP Settings）
  - Assets/Audio/DemoModeA/（UI SFX）
  - Assets/Animations/DemoModeA/（Animator/Clips/Override）
- 占位资源清单（最小集）
  - 情绪图标：5 枚（疲惫/精力充沛/烦躁/兴奋/重置）
  - 气泡预制体：PlayerBubble.prefab、CharacterBubble.prefab
  - Animator Controller：CharacterDemo.controller（绑定 AnimSpeed、PoseId）
  - 角色预制体：Character_Demo.prefab（必要组件就位）
  - VFX：星星眼特效、轻微“金粉”粒子、Reset 闪光/深呼吸特效（简易版）
  - Audio SFX：啧舌音、打字/提示音（可选）
  - PostProcessing：URP Volume Profile（低饱和 + 暗角 / 高对比 + 微粒子），或单一 Profile + 多组 Override
  - TMP 资源：Emoji Sprite Asset（预打包常用表情贴图，避免运行时报错）

## 11. 开发里程碑与任务拆解（建议 6 个迭代）
- M1 基础搭建（0.5 天）：新场景、角色预设、UI 框架、事件管道。
- M2 情绪系统（0.5 天）：EmotionType/EmotionProfile/EmotionController + 记忆计时。
- M3 多维反馈（1 天）：Presenter 驱动动画速度/姿态、UI 动效、Emoji 密度控制。
- M4 对话规则引擎（0.5 天）：基于情绪的文本生成与注意力策略。
- M5 打磨（0.5 天）：参数调优、极端输入处理、边界测试（快速切换/并发点击）。
- M6 验收与演示（0.5 天）：脚本化演示流、场景清理、打包或录屏。

## 12. 测试与验收清单
- 切换四种情绪，7 维度参数均在 200ms 内更新（Profile 调参记录）。
- 文案风格差异：逐条检查“语速描述/句式长短/Emoji 密度/注意力行为”。
- 情绪记忆：设置后 5 分钟内任意消息均保持风格；计时到期回中立；重置立即生效。
- 打字与标点：不同情绪的 TypingSpeed/ResponseDelay/PunctuationStyle 可感知；支持快进；无阻塞。
- 环境联动：Volume 混合与粒子按情绪正确变化；重置效果明显可见。
- 小动作：IdleRandomActions 按权重/冷却触发；无连发/无卡帧；可被新状态打断。
- 极端点击：在过渡中反复点击情绪，前序过渡被优雅中断并切换至新状态，无“瞬移/卡顿”。
- 资源健壮性：Emoji Sprite Asset 正常渲染，无紫块；缺失动画时降级逻辑生效。

## 13. 未来扩展
- 接入大语言模型与 TTS，实现更自然的“语音 + 文本”表达。
- 扩展情绪维度与角色个性参数，支持多人设切换与对比。
- 数据驱动（ScriptableObject）暴露调参界面，支持现场快速试错与打磨。

---
产出物：本计划文档、可运行的 DemoModeA 场景、角色与 UI 预设、脚本与配置，满足“情绪一键切换、7 维同步反馈、对话个性化、5 分钟记忆、可一眼感知差异”的演示目标。
