using System;
using UnityEngine;

namespace DemoModeA
{
    public class EmotionController : MonoBehaviour, IEmotionController
    {
        [SerializeField] private MonoBehaviour _profileRepositorySource;
        [SerializeField] private MonoBehaviour[] _emotionChangeListenerSource;

        [SerializeField] private EmotionType _initialEmotion = EmotionType.Neutral;
        [SerializeField] private float _memoryMinutes = 5f;
        [SerializeField] private bool _enableLogs = true;
        private IEmotionProfileRepository _profileRepository;
        private IEmotionChangeListener[] _emotionChangeListener;

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
            
            if (_enableLogs)
            {
                Debug.Log($"[{nameof(EmotionController)}] Awake. InitialEmotion={_initialEmotion}, MemoryMinutes={_memoryMinutes}, Repo={(_profileRepository != null ? _profileRepository.GetType().Name : "null")}", this);
                if (_profileRepository == null)
                {
                    Debug.LogError($"[{nameof(EmotionController)}] No IEmotionProfileRepository found. Please assign ProfileRepositorySource.", this);
                }
                Debug.Log($"[{nameof(EmotionController)}] EmotionChangeListeners={(_emotionChangeListener != null ? _emotionChangeListener.Length : 0)}", this);
            }

            initEmotionChangeListeners();

            _currentEmotion = _initialEmotion;
            refreshExpire();
        }

        private void Update()
        {
            if (_expireAt.HasValue && DateTime.UtcNow >= _expireAt.Value)
            {
                if (_currentEmotion != EmotionType.Neutral)
                {
                    if (_enableLogs)
                    {
                        Debug.Log($"[{nameof(EmotionController)}] Emotion expired at {_expireAt:O}. Resetting to Neutral.", this);
                    }
                    SetEmotion(EmotionType.Neutral);
                }
            }
        }

        public void SetEmotion(EmotionType emotion)
        {
            if (Time.unscaledTime - _lastChangeTime < MinSwitchInterval)
            {
                if (_enableLogs)
                {
                    Debug.Log($"[{nameof(EmotionController)}] SetEmotion ignored (rate limit). Requested={emotion}, Current={_currentEmotion}", this);
                }
                return;
            }
            if (emotion == _currentEmotion)
            {
                if (_enableLogs)
                {
                    Debug.Log($"[{nameof(EmotionController)}] SetEmotion ignored (same emotion). Emotion={emotion}", this);
                }
                return;
            }
            var old = _currentEmotion;
            _currentEmotion = emotion;
            _lastChangeTime = Time.unscaledTime;
            refreshExpire();
            OnEmotionChanged?.Invoke(old, _currentEmotion);
            var profile = _profileRepository?.GetProfile(_currentEmotion);
            if (_enableLogs)
            {
                Debug.Log($"[{nameof(EmotionController)}] Emotion changed: {old} -> {_currentEmotion}. ExpireAt={_expireAt:O}. Profile={(profile != null ? profile.name : "null")}", this);
            }
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

        private void initEmotionChangeListeners()
        {
            if (_emotionChangeListenerSource != null && _emotionChangeListenerSource.Length > 0)
            {
                var count = 0;
                for (int i = 0; i < _emotionChangeListenerSource.Length; i++)
                {
                    var mb = _emotionChangeListenerSource[i];
                    if (mb is IEmotionChangeListener)
                    {
                        count++;
                    }
                }

                if (count == 0)
                {
                    _emotionChangeListener = Array.Empty<IEmotionChangeListener>();
                    if (_enableLogs)
                    {
                        Debug.LogWarning($"[{nameof(EmotionController)}] EmotionChangeListenerSource set, but no element implements IEmotionChangeListener.", this);
                    }
                    return;
                }

                _emotionChangeListener = new IEmotionChangeListener[count];
                var write = 0;
                for (int i = 0; i < _emotionChangeListenerSource.Length; i++)
                {
                    var mb = _emotionChangeListenerSource[i];
                    if (mb is IEmotionChangeListener listener)
                    {
                        _emotionChangeListener[write] = listener;
                        write++;
                    }
                    else if (_enableLogs && mb != null)
                    {
                        Debug.LogWarning($"[{nameof(EmotionController)}] EmotionChangeListenerSource element does not implement IEmotionChangeListener: {mb.GetType().Name}", mb);
                    }
                }
                return;
            }

            _emotionChangeListener = GetComponentsInChildren<IEmotionChangeListener>(true);
            if (_emotionChangeListener == null)
            {
                _emotionChangeListener = Array.Empty<IEmotionChangeListener>();
            }
        }

        private void broadcastProfile(EmotionType oldEmotion, EmotionType newEmotion, EmotionProfile profile)
        {
            if (_enableLogs)
            {
                Debug.Log($"[{nameof(EmotionController)}] Broadcasting profile to {_emotionChangeListener.Length} listener(s). Emotion={newEmotion}, Profile={(profile != null ? profile.name : "null")}", this);
            }

            foreach (var iEmotionChangeListener in _emotionChangeListener)
            {
                iEmotionChangeListener.OnEmotionChanged(oldEmotion, newEmotion, profile);
            }
        }
    }
}
