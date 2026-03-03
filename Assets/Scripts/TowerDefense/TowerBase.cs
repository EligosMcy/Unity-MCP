using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace TowerDefense
{
    public class TowerBase : MonoBehaviour
    {
        [SerializeField] private GameObject _floorPrefab;
        [SerializeField] private Transform _floorParent;
        [SerializeField] private int _initialFloors = 3;
        [SerializeField] private float _floorHeight = 3f;
        [SerializeField] private float _dropSpeed = 5f;

        private List<TowerFloor> _floors = new List<TowerFloor>();
        private UnityEvent<int> _onFloorDestroyed;

        public int FloorCount => _floors.Count;

        private void Awake()
        {
            _onFloorDestroyed = new UnityEvent<int>();
            _onFloorDestroyed.AddListener(OnFloorDestroyed);

            if (_floorPrefab == null)
            {
                _floorPrefab = Resources.Load<GameObject>("Prefabs/TowerFloor");
            }

            if (_floorParent == null)
            {
                Transform child = transform.Find("Floors");
                if (child != null)
                {
                    _floorParent = child;
                }
                else
                {
                    GameObject floorsObj = new GameObject("Floors");
                    floorsObj.transform.SetParent(transform);
                    floorsObj.transform.localPosition = Vector3.zero;
                    _floorParent = floorsObj.transform;
                }
            }
        }

        private void Start()
        {
            for (int i = 0; i < _initialFloors; i++)
            {
                CreateFloor(i);
            }
        }

        public void CreateFloor(int index)
        {
            if (_floorPrefab == null)
            {
                Debug.LogError("TowerFloor prefab not assigned!");
                return;
            }

            GameObject floorObj = Instantiate(_floorPrefab, _floorParent);
            float yPosition = index * _floorHeight;
            floorObj.transform.localPosition = new Vector3(0, yPosition, 0);

            TowerFloor floor = floorObj.GetComponent<TowerFloor>();
            floor.Initialize(index, _onFloorDestroyed);
            
            _floors.Add(floor);
            UpdateFloorIndices();
        }

        private void OnFloorDestroyed(int destroyedIndex)
        {
            _floors.RemoveAll(f => f.FloorIndex == destroyedIndex);
            DropFloors();
            UpdateFloorIndices();
        }

        private void DropFloors()
        {
            foreach (TowerFloor floor in _floors)
            {
                if (floor.FloorIndex > 0)
                {
                    int newIndex = floor.FloorIndex - 1;
                    Vector3 targetPosition = new Vector3(0, newIndex * _floorHeight, 0);
                    StartCoroutine(DropFloorRoutine(floor, targetPosition));
                }
            }
        }

        private System.Collections.IEnumerator DropFloorRoutine(TowerFloor floor, Vector3 targetPosition)
        {
            Vector3 startPosition = floor.transform.localPosition;
            float t = 0;
            
            while (t < 1)
            {
                t += Time.deltaTime * _dropSpeed;
                floor.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            floor.transform.localPosition = targetPosition;
        }

        private void UpdateFloorIndices()
        {
            for (int i = 0; i < _floors.Count; i++)
            {
                _floors[i].Initialize(i, _onFloorDestroyed);
            }
        }

        public int GetBuildCost(int floorIndex)
        {
            return (int)Mathf.Pow(2, floorIndex);
        }

        public List<TowerFloor> GetAllFloors()
        {
            return new List<TowerFloor>(_floors);
        }
    }
}
