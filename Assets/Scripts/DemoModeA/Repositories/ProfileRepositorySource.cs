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
                var loaded = Resources.LoadAll<EmotionProfile>("DemoModeA");
                if (loaded != null)
                {
                    for (int i = 0; i < loaded.Length; i++)
                    {
                        var p = loaded[i];
                        if (p != null && !_map.ContainsKey(p.Emotion))
                        {
                            _map.Add(p.Emotion, p);
                        }
                    }
                }
            }
            if (_map.Count == 0)
            {
                var repo = FindFirstObjectByType<EmotionProfileRepository>();
                if (repo != null)
                {
                    var types = new[] { EmotionType.Neutral, EmotionType.Tired, EmotionType.Energetic, EmotionType.Irritable, EmotionType.Excited };
                    for (int i = 0; i < types.Length; i++)
                    {
                        var p = repo.GetProfile(types[i]);
                        if (p != null && !_map.ContainsKey(types[i]))
                        {
                            _map.Add(types[i], p);
                        }
                    }
                }
            }
        }
    }
}
