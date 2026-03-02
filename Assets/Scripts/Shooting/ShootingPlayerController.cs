using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingPlayerController : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject missEffectPrefab;
    [SerializeField] private float missEffectDistance = 5.0f; // 击空时特效生成的距离

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (_mainCamera == null) return;

            bool isHit = false;
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Ball ball = hit.collider.GetComponent<Ball>();
                if (ball != null)
                {
                    ball.OnHit();
                    isHit = true;
                    SpawnEffect(hitEffectPrefab, hit.point);
                }
                else
                {
                    // 击中了其他物体（如墙壁）
                    SpawnEffect(missEffectPrefab, hit.point);
                }
            }
            else
            {
                // 击空（射向天空/虚空）
                Vector3 missPos = ray.origin + ray.direction * missEffectDistance;
                SpawnEffect(missEffectPrefab, missPos);
            }

            // 记录点击结果
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterClick(isHit);
            }
        }
    }

    private void SpawnEffect(GameObject prefab, Vector3 position)
    {
        if (prefab != null)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}