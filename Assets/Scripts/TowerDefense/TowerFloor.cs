using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense
{
    public class TowerFloor : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;

        private int _currentHealth;
        private int _floorIndex;
        private UnityEvent<int> _onFloorDestroyed;
        private Turret _turret;

        public int FloorIndex => _floorIndex;
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        private void Awake()
        {
            _turret = GetComponentInChildren<Turret>();
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
                defaultMat.color = new Color(0.4f, 0.6f, 0.9f);
                meshRenderer.sharedMaterial = defaultMat;
            }

            CreateDirectionIndicator();
        }

        private void CreateDirectionIndicator()
        {
            Transform firePoint = transform.Find("Turret/FirePoint");
            if (firePoint == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.name == "Turret")
                    {
                        firePoint = child.Find("FirePoint");
                        break;
                    }
                }
            }

            if (firePoint != null)
            {
                GameObject directionObj = new GameObject("DirectionIndicator");
                directionObj.transform.SetParent(firePoint);
                directionObj.transform.localPosition = Vector3.zero;
                directionObj.transform.localScale = Vector3.one * 0.3f;

                MeshFilter mf = directionObj.AddComponent<MeshFilter>();
                mf.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

                MeshRenderer mr = directionObj.AddComponent<MeshRenderer>();
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = Color.red;
                mr.sharedMaterial = mat;
            }
        }

        public void Initialize(int floorIndex, UnityEvent<int> onFloorDestroyed)
        {
            _floorIndex = floorIndex;
            _onFloorDestroyed = onFloorDestroyed;
            _currentHealth = _maxHealth;

            if (_turret == null)
            {
                _turret = GetComponentInChildren<Turret>();
            }

            if (_turret != null)
            {
                _turret.SetShowRangeIndicator(_floorIndex == 0);
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                DestroyFloor();
            }
        }

        private void DestroyFloor()
        {
            _onFloorDestroyed?.Invoke(_floorIndex);
            Destroy(gameObject);
        }
    }
}
