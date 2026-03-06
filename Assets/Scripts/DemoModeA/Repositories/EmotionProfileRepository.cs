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
                if (_map.Count == 0)
                {
                    var neutral = ScriptableObject.CreateInstance<EmotionProfile>();
                    configure(neutral, EmotionType.Neutral, 1.0f, 0.7f, TextLengthBias.Medium, AffirmationBias.Neutral, 0.1f, new[] { "🙂", "😉" }, 0, 1.0f, 0.5f, AttentionMode.Focused, 12f, 0.2f, PunctuationStyle.Neutral, VisualMoodPreset.Neutral, 0.5f);
                    var tired = ScriptableObject.CreateInstance<EmotionProfile>();
                    configure(tired, EmotionType.Tired, 0.7f, 0.5f, TextLengthBias.Short, AffirmationBias.Neutral, 0.05f, new[] { "…", "😪", "😴" }, 1, 0.6f, 0.3f, AttentionMode.Random, 7f, 1.5f, PunctuationStyle.EllipsisHeavy, VisualMoodPreset.LowSaturationVignette, 0.7f);
                    var energetic = ScriptableObject.CreateInstance<EmotionProfile>();
                    configure(energetic, EmotionType.Energetic, 1.2f, 0.8f, TextLengthBias.Medium, AffirmationBias.Positive, 0.3f, new[] { "😊", "✨" }, 2, 1.0f, 0.7f, AttentionMode.Focused, 13f, 0.4f, PunctuationStyle.Neutral, VisualMoodPreset.HighContrastSparkle, 0.4f);
                    var irritable = ScriptableObject.CreateInstance<EmotionProfile>();
                    configure(irritable, EmotionType.Irritable, 0.9f, 0.7f, TextLengthBias.Short, AffirmationBias.Negative, 0.05f, new[] { "😒", "🙄" }, 3, 0.6f, 0.3f, AttentionMode.Reject, 11f, 0.8f, PunctuationStyle.Minimal, VisualMoodPreset.LowSaturationVignette, 0.6f);
                    var excited = ScriptableObject.CreateInstance<EmotionProfile>();
                    configure(excited, EmotionType.Excited, 1.4f, 0.9f, TextLengthBias.Long, AffirmationBias.Positive, 0.8f, new[] { "🎉", "🥳", "🚀" }, 4, 1.6f, 0.9f, AttentionMode.Focused, 18f, 0.1f, PunctuationStyle.Exclamatory, VisualMoodPreset.HighContrastSparkle, 0.6f);
                    _map[EmotionType.Neutral] = neutral;
                    _map[EmotionType.Tired] = tired;
                    _map[EmotionType.Energetic] = energetic;
                    _map[EmotionType.Irritable] = irritable;
                    _map[EmotionType.Excited] = excited;
                }
            }
        }

        private void configure(EmotionProfile p, EmotionType type, float rate, float volume, TextLengthBias tl, AffirmationBias af, float emojiDensity, string[] emojis, int pose, float animSpeed, float intensity, AttentionMode att, float typing, float delay, PunctuationStyle punct, VisualMoodPreset preset, float volumeBlend)
        {
            p.Emotion = type;
            p.VoiceRate = rate;
            p.VoiceVolume = volume;
            p.TextLengthBias = tl;
            p.AffirmationBias = af;
            p.EmojiDensity = emojiDensity;
            p.EmojiPool = emojis;
            p.PoseId = pose;
            p.LayerWeight = 1f;
            p.AnimSpeed = animSpeed;
            p.ExpressIntensity = intensity;
            p.AttentionMode = att;
            p.TypingSpeed = typing;
            p.ResponseDelay = delay;
            p.PunctuationStyle = punct;
            p.VisualPreset = preset;
            p.VolumeBlendWeight = volumeBlend;
        }
    }
}
