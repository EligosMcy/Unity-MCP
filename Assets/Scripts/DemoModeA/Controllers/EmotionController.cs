using System;
using UnityEngine;

namespace DemoModeA
{
    public class EmotionController : MonoBehaviour, IEmotionController
    {
        [SerializeField] private MonoBehaviour _profileRepositorySource;
        [SerializeField] private EmotionType _initialEmotion = EmotionType.Neutral;
        [SerializeField] private float _memoryMinutes = 5f;
        private IEmotionProfileRepository _profileRepository;

        private EmotionType _currentEmotion;
        private DateTime? _expireAt;
        private float _lastChangeTime;
        private const float MinSwitchInterval = 0.1f;

        public event Action<EmotionType, EmotionType> OnEmotionChanged;

        public EmotionType CurrentEmotion => _currentEmotion;
        public DateTime? ExpireAt => _expireAt;

        private void Awake()
        {
            _profileRepository = _profileRepositorySource as IEmotionProfileRepository;
            if (_profileRepository == null)
            {
                var monos = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                for (int i = 0; i < monos.Length; i++)
                {
                    if (monos[i] is IEmotionProfileRepository r)
                    {
                        _profileRepository = r;
                        break;
                    }
                }
            }
            _currentEmotion = _initialEmotion;
            refreshExpire();
        }

        private void Update()
        {
            if (_expireAt.HasValue && DateTime.UtcNow >= _expireAt.Value)
            {
                if (_currentEmotion != EmotionType.Neutral)
                {
                    SetEmotion(EmotionType.Neutral);
                }
            }
        }

        public void SetEmotion(EmotionType emotion)
        {
            if (Time.unscaledTime - _lastChangeTime < MinSwitchInterval) return;
            if (emotion == _currentEmotion) return;
            var old = _currentEmotion;
            _currentEmotion = emotion;
            _lastChangeTime = Time.unscaledTime;
            refreshExpire();
            OnEmotionChanged?.Invoke(old, _currentEmotion);
            var profile = _profileRepository != null ? _profileRepository.GetProfile(_currentEmotion) : null;
            broadcastProfile(old, _currentEmotion, profile);
        }

        public void ResetEmotion()
        {
            SetEmotion(EmotionType.Neutral);
        }

        public float GetRemainingTime()
        {
            if (!_expireAt.HasValue) return 0f;
            var seconds = (float)(_expireAt.Value - DateTime.UtcNow).TotalSeconds;
            return Mathf.Max(0f, seconds);
        }

        private void refreshExpire()
        {
            _expireAt = DateTime.UtcNow.AddMinutes(_memoryMinutes);
        }

        private void broadcastProfile(EmotionType oldEmotion, EmotionType newEmotion, EmotionProfile profile)
        {
            var listeners = GetComponentsInChildren<IEmotionChangeListener>(true);
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].OnEmotionChanged(oldEmotion, newEmotion, profile);
            }
        }
    }
}
