using UnityEngine;
using System.Collections;

namespace TowerDefense
{
    public class MonsterSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _monsterPrefab;
        [SerializeField] private Transform _towerTarget;
        [SerializeField] private float _spawnInterval = 3f;
        [SerializeField] private int _maxMonsters = 10;
        [SerializeField] private float _spawnRadius = 25f;

        private int _currentMonsters;

        private void Awake()
        {

        }

        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_spawnInterval);

                if (_currentMonsters < _maxMonsters)
                {
                    SpawnMonster();
                }
            }
        }

        private void SpawnMonster()
        {
            if (_monsterPrefab != null && _towerTarget != null)
            {
                Vector2 randomCircle = Random.insideUnitCircle.normalized * _spawnRadius;
                Vector3 spawnPosition = new Vector3(
                    _towerTarget.position.x + randomCircle.x,
                    0.5f,
                    _towerTarget.position.z + randomCircle.y
                );

                GameObject monsterObj = Instantiate(_monsterPrefab, spawnPosition, Quaternion.identity);
                Monster monster = monsterObj.GetComponent<Monster>();

                if (monster != null)
                {
                    monster.Initialize(_towerTarget);
                }

                _currentMonsters++;
            }
        }

        public void OnMonsterDestroyed()
        {
            _currentMonsters--;
        }
    }
}
