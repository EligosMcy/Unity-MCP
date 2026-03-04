using UnityEngine;

namespace TowerDefense
{
    public class Turret : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 3600f;
        [SerializeField] private float _attackRange = 15f;
        [SerializeField] private float _attackInterval = 1f;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Material _rangeIndicatorMaterial;
        [SerializeField] private bool _showRangeIndicator = true;
        [SerializeField] private float _aimThreshold = 0.99f;

        private float _attackTimer;
        private float _findTargetTimer;
        private const float FIND_TARGET_INTERVAL = 0.2f;
        private Transform _target;
        private GameObject _rangeIndicator;
        private Quaternion _idleRotation;

        public float AttackRange => _attackRange;

        public void SetShowRangeIndicator(bool show)
        {
            _showRangeIndicator = show;
            if (_rangeIndicator != null)
            {
                _rangeIndicator.SetActive(show);
            }
        }

        private void Awake()
        {
            setupMesh();

            if (_firePoint == null)
            {
                _firePoint = transform.Find("FirePoint");
            }
            if (_firePoint == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.name.Contains("Fire"))
                    {
                        _firePoint = child;
                        break;
                    }
                }
            }

            if (_rangeIndicatorMaterial == null)
            {
                _rangeIndicatorMaterial = Resources.Load<Material>("Materials/RangeIndicatorMaterial");
            }

            _idleRotation = transform.rotation;
        }

        private void setupMesh()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            if (meshFilter != null && meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            }

            if (meshRenderer != null && (meshRenderer.sharedMaterials == null || meshRenderer.sharedMaterials.Length == 0 || meshRenderer.sharedMaterials[0] == null))
            {
                Material defaultMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                defaultMat.color = Color.gray;
                meshRenderer.sharedMaterial = defaultMat;
            }
        }

        private void Start()
        {
            _attackTimer = _attackInterval;
            if (_showRangeIndicator)
            {
                createRangeIndicator();
            }
        }

        private void createRangeIndicator()
        {
            _rangeIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _rangeIndicator.name = "RangeIndicator";
            _rangeIndicator.transform.SetParent(transform);
            _rangeIndicator.transform.localPosition = new Vector3(0, -1.5f + 0.005f, 0);
            _rangeIndicator.transform.localRotation = Quaternion.identity;
            
            float scale = _attackRange * 2f;
            _rangeIndicator.transform.localScale = new Vector3(scale, 0.002f, scale);

            MeshRenderer renderer = _rangeIndicator.GetComponent<MeshRenderer>();
            
            if (_rangeIndicatorMaterial != null)
            {
                renderer.material = _rangeIndicatorMaterial;
            }
            else
            {
                Material fallbackMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                fallbackMat.SetFloat("_Surface", 1);
                fallbackMat.SetFloat("_Blend", 1);
                fallbackMat.color = new Color(0, 0.5f, 1f, 0.3f);
                renderer.material = fallbackMat;
            }

            Collider collider = _rangeIndicator.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
        }

        private void Update()
        {
            _attackTimer -= Time.deltaTime;
            _findTargetTimer -= Time.deltaTime;

            if (_findTargetTimer <= 0)
            {
                findTarget();
                _findTargetTimer = FIND_TARGET_INTERVAL;
            }
            
            if (_target != null)
            {
                bool isFacingTarget = rotateTowardsTarget();
                
                if (isFacingTarget && _attackTimer <= 0)
                {
                    fire();
                    _attackTimer = _attackInterval;
                }
            }
        }

        private void findTarget()
        {
            if (_target != null)
            {
                float distance = Vector3.Distance(transform.position, _target.position);
                if (distance > _attackRange)
                {
                    _target = null;
                    _idleRotation = transform.rotation;
                }
            }

            if (_target == null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRange);
                float closestDistance = float.MaxValue;
                Transform closestTarget = null;

                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("Monster"))
                    {
                        float distance = Vector3.Distance(transform.position, collider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTarget = collider.transform;
                        }
                    }
                }

                _target = closestTarget;
            }
        }

        private bool rotateTowardsTarget()
        {
            if (_target != null)
            {
                Vector3 direction = _target.position - transform.position;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        _rotationSpeed * Time.deltaTime
                    );

                    return Quaternion.Dot(transform.rotation, targetRotation) > _aimThreshold;
                }
            }
            return false;
        }

        private void fire()
        {
            if (_target != null && BulletPool.Instance != null)
            {
                Vector3 firePosition = _firePoint != null ? _firePoint.position : transform.position + transform.forward * 0.5f;
                Bullet bullet = BulletPool.Instance.GetBullet();
                if (bullet != null)
                {
                    bullet.transform.position = firePosition;
                    Vector3 direction = (_target.position - firePosition).normalized;
                    bullet.Initialize(direction);
                }
            }
        }
    }
}
