using System.Collections.Generic;
using UnityEngine;

namespace DemoModeA
{
    public class ProfileRepositorySource : MonoBehaviour, IEmotionProfileRepository
    {
        [SerializeField] private EmotionProfileSet[] _sets = new EmotionProfileSet[0];
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
            if (_sets != null)
            {
                for (int s = 0; s < _sets.Length; s++)
                {
                    var set = _sets[s];
                    if (set == null || set.Entries == null) continue;
                    for (int i = 0; i < set.Entries.Length; i++)
                    {
                        var e = set.Entries[i];
                        if (!_map.ContainsKey(e.Emotion) && e.Profile != null)
                        {
                            _map.Add(e.Emotion, e.Profile);
                        }
                    }
                }
            }
            
            if (_map.Count == 0)
            {
                Debug.LogError($"[{nameof(ProfileRepositorySource)}] No profile sets configured or sets are empty. Please assign EmotionProfileSet assets.", this);
            }
        }
    }
}
