using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class SnakeBodyManager : MonoBehaviour
{
    [Header("引用设置")]
    [SerializeField]
    private GameSettingScriptableObject _gameSetting;

    [Header("身体设置")]
    [SerializeField]
    private List<GameObject> _bodyParts = new List<GameObject>();

    [SerializeField]
    private GameObject _head;

    private GameObject _bodyPrefab;
    private float _boundary;

    // 动画相关
    private bool _isColorChanging = false;
    private Color _targetColor = Color.white;

    // 事件
    public delegate void BodyAnimationComplete();
    public event BodyAnimationComplete OnBodyAnimationComplete;

    public void Initialize(GameSettingScriptableObject gameSetting, GameObject head)
    {
        _gameSetting = gameSetting;
        _head = head;
        _bodyPrefab = _gameSetting.BodyPrefab;
        _boundary = _gameSetting.Boundary;

        // 初始化身体
        ResetBody();
    }

    void Start()
    {
        // 初始调整身体大小
        AdjustBodySizes();
    }

    public List<GameObject> GetBodyParts()
    {
        return _bodyParts;
    }

    public void ResetBody()
    {
        // 清除所有身体段
        foreach (GameObject bodyPart in _bodyParts)
        {
            Destroy(bodyPart);
        }
        _bodyParts.Clear();

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
        body1.tag = _gameSetting.BodyTag;
        body1.transform.localPosition = new Vector3(-1, 0, 0);

        _bodyParts.Add(body1);

        // 调整身体大小
        AdjustBodySizes();
    }

    public void AddBodyPart(Vector3 position)
    {
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
        newBodyPart.tag = _gameSetting.BodyTag;
        newBodyPart.transform.localPosition = position;

        _bodyParts.Add(newBodyPart);

        // 调整身体大小
        AdjustBodySizes();
    }

    public void AddMultipleBodyParts(Vector3 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddBodyPart(position);
        }
    }

    public void MoveBodyParts(Vector3 targetPosition, Vector3 previousHeadPosition, float moveInterval)
    {
        if (_bodyParts.Count > 0)
        {
            // 依次移动每个身体段
            Vector3 previousPosition = previousHeadPosition;
            
            for (int i = 0; i < _bodyParts.Count; i++)
            {
                GameObject bodyPart = _bodyParts[i];
                if (bodyPart != null)
                {
                    // 记录当前身体段的位置
                    Vector3 currentPosition = bodyPart.transform.localPosition;
                    
                    // 使用 DoTween 移动身体段
                    bodyPart.transform.DOLocalMove(previousPosition, moveInterval).SetEase(Ease.Linear);
                    
                    // 更新前一个位置为当前身体段的位置
                    previousPosition = currentPosition;
                }
            }
        }

        // 调整身体大小
        AdjustBodySizes();
    }

    public void AdjustBodySizes()
    {
        int bodyCount = _bodyParts.Count;
        if (bodyCount == 0)
            return;

        // 计算需要逐渐变小的身体段数量
        int shrinkCount = Mathf.Max(1, Mathf.FloorToInt((_gameSetting.MaxBodySize - _gameSetting.MinBodySize) / _gameSetting.SizeDecrement) + 1);
        shrinkCount = Mathf.Min(shrinkCount, bodyCount);

        // 从头部开始往后逐渐变小
        for (int i = 0; i < bodyCount; i++)
        {
            GameObject bodyPart = _bodyParts[i];
            if (bodyPart != null)
            {
                if (i < shrinkCount)
                {
                    // 逐渐变小
                    float scaleFactor = _gameSetting.MaxBodySize - (i * _gameSetting.SizeDecrement);

                    // 确保不小于最小尺寸
                    scaleFactor = Mathf.Max(scaleFactor, _gameSetting.MinBodySize);
                    bodyPart.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                }
                else
                {
                    // 最小大小
                    bodyPart.transform.localScale = new Vector3(_gameSetting.MinBodySize, _gameSetting.MinBodySize, _gameSetting.MinBodySize);
                }
            }
        }
    }

    public void StartColorChangeAnimation(Color targetColor)
    {
        if (!_isColorChanging)
        {
            _targetColor = targetColor;
            _isColorChanging = true;
            StartCoroutine(ColorChangeAnimationCoroutine());
        }
    }

    private IEnumerator ColorChangeAnimationCoroutine()
    {
        // 计算每个身体段的变色间隔
        float colorChangeInterval = _gameSetting.ColorChangeDuration / (_bodyParts.Count * 2); // 每个身体段有变色和还原两个步骤

        // 为每个身体段依次变色
        for (int i = 0; i < _bodyParts.Count; i++)
        {
            // 为当前身体段变色
            GameObject bodyPart = _bodyParts[i];
            if (bodyPart != null)
            {
                Renderer renderer = bodyPart.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 使用 DOTween 实现平滑颜色过渡
                    renderer.material.DOColor(_targetColor, colorChangeInterval);
                }
            }

            // 等待变色完成
            yield return new WaitForSeconds(colorChangeInterval);

            // 还原当前身体段的颜色
            if (bodyPart != null)
            {
                Renderer renderer = bodyPart.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 使用 DOTween 实现平滑颜色过渡
                    renderer.material.DOColor(_gameSetting.OriginalColor, colorChangeInterval);
                }
            }

            // 等待还原完成
            yield return new WaitForSeconds(colorChangeInterval);
        }

        // 所有身体段都变色并还原后，添加新身体段
        if (_bodyParts.Count > 0)
        {
            Vector3 newBodyPosition = _bodyParts[_bodyParts.Count - 1].transform.localPosition;
            AddBodyPart(newBodyPosition);

            // 为新身体段设置原始颜色
            if (_bodyParts.Count > 0)
            {
                GameObject newBodyPart = _bodyParts[_bodyParts.Count - 1];
                if (newBodyPart != null)
                {
                    Renderer renderer = newBodyPart.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = _gameSetting.OriginalColor;
                    }
                }
            }
        }

        _isColorChanging = false;

        // 触发动画完成事件
        OnBodyAnimationComplete?.Invoke();
    }
}
