using System.Collections.Generic;
using UnityEngine;

namespace DemoModeA
{
    public class EmotionProfileRepository : MonoBehaviour, IEmotionProfileRepository
    {
        [System.Serializable]
        public struct Entry
        {
            public EmotionType Emotion;
            public EmotionProfile Profile;
        }

        [SerializeField] private Entry[] _entries = new Entry[0];
        private Dictionary<EmotionType, EmotionProfile> _map;

        private void Awake()
        {
            buildMap();
        }

        public EmotionProfile GetProfile(EmotionType emotion)
        {
            if (_map == null) buildMap();
            if (_map != null && _map.TryGetValue(emotion, out var p)) return p;
            return null;
        }

        private void buildMap()
        {
            _map = new Dictionary<EmotionType, EmotionProfile>();
            var hasEntries = _entries != null && _entries.Length > 0;
            if (hasEntries)
            {
                for (int i = 0; i < _entries.Length; i++)
                {
                    var e = _entries[i];
                    if (!_map.ContainsKey(e.Emotion) && e.Profile != null)
                    {
                        _map.Add(e.Emotion, e.Profile);
                    }
                }
            }
            else
            {
                Debug.LogError($"[{nameof(EmotionProfileRepository)}] No entries configured. Please assign emotion profiles in inspector.", this);
            }
        }
    }
}
