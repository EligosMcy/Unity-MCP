using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense
{
    public class TowerDefenseGameManager : MonoBehaviour
    {
        public static TowerDefenseGameManager Instance { get; private set; }

        [SerializeField] private int _initialGold = 1;
        [SerializeField] private int _killReward = 10;
        [SerializeField] private TowerBase _towerBase;
        [SerializeField] private MonsterSpawner _spawner;
        [SerializeField] private GameUI _gameUI;

        private int _gold;
        private bool _isGameRunning = true;

        public int Gold => _gold;
        public bool IsGameRunning => _isGameRunning;

        public UnityEvent<int> _onGoldChanged;
        public UnityEvent _onGameOver;

        private void Awake()
        {
            Instance = this;
            _onGoldChanged = new UnityEvent<int>();
            _onGameOver = new UnityEvent();
        }

        private void Start()
        {
            _gold = _initialGold;
            UpdateGoldUI();
        }

        public void AddGold(int amount)
        {
            _gold += amount;
            UpdateGoldUI();
        }

        public bool SpendGold(int amount)
        {
            if (_gold >= amount)
            {
                _gold -= amount;
                UpdateGoldUI();
                return true;
            }
            return false;
        }

        private void UpdateGoldUI()
        {
            _onGoldChanged?.Invoke(_gold);
            if (_gameUI != null)
            {
                _gameUI.UpdateGold(_gold);
            }
        }

        public void OnMonsterKilled()
        {
            AddGold(_killReward);
            if (_spawner != null)
            {
                _spawner.OnMonsterDestroyed();
            }
        }

        public void BuildNewFloor()
        {
            if (_towerBase != null)
            {
                int floorIndex = _towerBase.FloorCount;
                int cost = _towerBase.GetBuildCost(floorIndex);
                
                if (SpendGold(cost))
                {
                    _towerBase.CreateFloor(floorIndex);
                }
            }
        }

        public int GetNextFloorCost()
        {
            if (_towerBase != null)
            {
                return _towerBase.GetBuildCost(_towerBase.FloorCount);
            }
            return 0;
        }

        public void ToggleGame()
        {
            _isGameRunning = !_isGameRunning;
            Time.timeScale = _isGameRunning ? 1 : 0;
        }

        public void GameOver()
        {
            _isGameRunning = false;
            Time.timeScale = 0;
            _onGameOver?.Invoke();
        }
    }
}
