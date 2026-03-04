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
        private float _attackTimer;
        private TowerFloor _targetFloor;
        private float _baseY;

        public int CurrentHealth => _currentHealth;

        private void Awake()
        {
            SetupMesh();
        }

        private void SetupMesh()
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
                // Calculate horizontal distance (ignore Y axis) to prevent jump height from affecting range check
                Vector3 horizontalPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 targetHorizontalPos = new Vector3(_targetTower.position.x, 0, _targetTower.position.z);
                float distance = Vector3.Distance(horizontalPos, targetHorizontalPos);
                
                if (distance > _attackRange)
                {
                    MoveTowardsTower();
                }
                else
                {
                    AttackTower();
                }
            }
        }

        private void MoveTowardsTower()
        {
            Vector3 direction = (_targetTower.position - transform.position).normalized;
            direction.y = 0;
            
            // Apply horizontal movement
            Vector3 newPos = transform.position + direction * _moveSpeed * Time.deltaTime;
            
            // Apply jump
            float jumpOffset = Mathf.Abs(Mathf.Sin(Time.time * _jumpFrequency)) * _jumpHeight;
            newPos.y = _baseY + jumpOffset;
            
            transform.position = newPos;
            
            // Look at target but keep upright
            Vector3 targetLookPos = _targetTower.position;
            targetLookPos.y = transform.position.y;
            transform.LookAt(targetLookPos);
        }

        private void AttackTower()
        {
            if (_attackTimer <= 0)
            {
                FindClosestFloor();
                if (_targetFloor != null)
                {
                    _targetFloor.TakeDamage(_attackDamage);
                    _attackTimer = _attackInterval;
                }
            }
        }

        private void FindClosestFloor()
        {
            TowerBase towerBase = _targetTower.GetComponent<TowerBase>();
            if (towerBase != null)
            {
                var floors = towerBase.GetAllFloors();
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
                Die();
            }
        }

        private void Die()
        {
            if (TowerDefenseGameManager.Instance != null)
            {
                TowerDefenseGameManager.Instance.OnMonsterKilled();
            }
            Destroy(gameObject);
        }
    }
}
