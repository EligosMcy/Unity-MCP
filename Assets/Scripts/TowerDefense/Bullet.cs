using UnityEngine;

namespace TowerDefense
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _lifetime = 3f;
        [SerializeField] private int _damage = 15;

        private Vector3 _direction;
        private float _timer;

        public int Damage => _damage;

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
                meshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            }

            if (meshRenderer != null && (meshRenderer.sharedMaterials == null || meshRenderer.sharedMaterials.Length == 0 || meshRenderer.sharedMaterials[0] == null))
            {
                Material defaultMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                defaultMat.color = Color.yellow;
                meshRenderer.sharedMaterial = defaultMat;
            }
        }

        public void Initialize(Vector3 direction)
        {
            _direction = direction.normalized;
            _timer = _lifetime;
            transform.rotation = Quaternion.LookRotation(_direction);
            transform.localScale = Vector3.one * 0.2f;
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;

            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Monster"))
            {
                Monster monster = other.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.TakeDamage(_damage);
                }
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            if (BulletPool.Instance != null)
            {
                BulletPool.Instance.ReturnBullet(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
