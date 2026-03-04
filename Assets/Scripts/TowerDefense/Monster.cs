using UnityEngine;

namespace TowerDefense
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _attackDamage = 10;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _jumpHeight = 2.0f;
        [SerializeField] private float _jumpFrequency = 5.0f;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackInterval = 1f;

        private int _currentHealth;
        private Transform _targetTower;
        private TowerBase _cachedTowerBase;
        private float _attackTimer;
        private TowerFloor _targetFloor;
        private float _baseY;

        public int CurrentHealth => _currentHealth;

        private void Awake()
        {
            setupMesh();
        }

        private void setupMesh()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            if (meshFilter != null && meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            }

            if (meshRenderer != null && (meshRenderer.sharedMaterials == null || meshRenderer.sharedMaterials.Length == 0 || meshRenderer.sharedMaterials[0] == null))
            {
                Material defaultMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                defaultMat.color = Color.red;
                meshRenderer.sharedMaterial = defaultMat;
            }
        }

        public void Initialize(Transform target)
        {
            _targetTower = target;
            if (_targetTower != null)
            {
                _cachedTowerBase = _targetTower.GetComponent<TowerBase>();
            }
            _currentHealth = _maxHealth;
            _attackTimer = 0;
            transform.localScale = Vector3.one * 1.5f;
            _baseY = transform.position.y;
        }

        private void Update()
        {
            _attackTimer -= Time.deltaTime;

            if (_targetTower != null)
            {
                // 计算水平距离（忽略Y轴），防止跳跃高度影响范围检查
                Vector3 horizontalPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 targetHorizontalPos = new Vector3(_targetTower.position.x, 0, _targetTower.position.z);
                float distance = Vector3.Distance(horizontalPos, targetHorizontalPos);
                
                if (distance > _attackRange)
                {
                    moveTowardsTower();
                }
                else
                {
                    attackTower();
                }
            }
        }

        private void moveTowardsTower()
        {
            Vector3 direction = (_targetTower.position - transform.position).normalized;
            direction.y = 0;
            
            // 应用水平移动
            Vector3 newPos = transform.position + direction * _moveSpeed * Time.deltaTime;
            
            // 应用跳跃
            float jumpOffset = Mathf.Abs(Mathf.Sin(Time.time * _jumpFrequency)) * _jumpHeight;
            newPos.y = _baseY + jumpOffset;
            
            transform.position = newPos;
            
            // 面向目标但保持直立
            Vector3 targetLookPos = _targetTower.position;
            targetLookPos.y = transform.position.y;
            transform.LookAt(targetLookPos);
        }

        private void attackTower()
        {
            if (_attackTimer <= 0)
            {
                findClosestFloor();
                if (_targetFloor != null)
                {
                    _targetFloor.TakeDamage(_attackDamage);
                    _attackTimer = _attackInterval;
                }
            }
        }

        private void findClosestFloor()
        {
            if (_cachedTowerBase != null)
            {
                var floors = _cachedTowerBase.GetAllFloors();
                float closestDistance = float.MaxValue;
                TowerFloor closestFloor = null;

                foreach (var floor in floors)
                {
                    float distance = Vector3.Distance(transform.position, floor.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestFloor = floor;
                    }
                }

                _targetFloor = closestFloor;
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                die();
            }
        }

        private void die()
        {
            if (TowerDefenseGameManager.Instance != null)
            {
                TowerDefenseGameManager.Instance.OnMonsterKilled();
            }
            Destroy(gameObject);
        }
    }
}
