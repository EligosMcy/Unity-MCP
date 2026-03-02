using UnityEngine;

public class Ball : MonoBehaviour
{
    private float _shrinkSpeed = 0.5f;
    private float _initialScale;
    private int _baseScore = 100;

    // 初始化时设置随机大小
    public void Init(float minScale, float maxScale, float shrinkSpeed)
    {
        _initialScale = Random.Range(minScale, maxScale);
        transform.localScale = Vector3.one * _initialScale;
        _shrinkSpeed = shrinkSpeed;
    }

    private void Update()
    {
        // 逐渐缩小
        transform.localScale -= Vector3.one * _shrinkSpeed * Time.deltaTime;

        // 如果缩小到0或以下，回收到对象池
        if (transform.localScale.x <= 0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ReturnBall(this);
            else
                Destroy(gameObject); // Fallback
        }
    }

    public void OnHit()
    {
        // 计算得分：基于当前大小与初始大小的比例
        // 初始时比例为1，得分最高
        // 快消失时比例接近0，得分最低
        if (GameManager.Instance != null)
        {
            float sizeRatio = transform.localScale.x / _initialScale;
            // 确保不为负数
            sizeRatio = Mathf.Max(0f, sizeRatio);
            
            // 最终分数 = 基础分数 * 大小比例
            int score = Mathf.CeilToInt(_baseScore * sizeRatio);
            GameManager.Instance.AddScore(score);

            GameManager.Instance.ReturnBall(this);
        }
        else
        {
            Destroy(gameObject); // Fallback
        }
    }
}