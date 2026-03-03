using UnityEngine;

namespace TowerDefense
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _attackDamage = 10;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackInterval = 1f;

        private int _currentHealth;
        private Transform _targetTower;
        private float _attackTimer;
        private TowerFloor _targetFloor;

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
        }

        private void Update()
        {
            _attackTimer -= Time.deltaTime;

            if (_targetTower != null)
            {
                float distance = Vector3.Distance(transform.position, _targetTower.position);
                
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
            transform.position += direction * _moveSpeed * Time.deltaTime;
            transform.LookAt(_targetTower);
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
