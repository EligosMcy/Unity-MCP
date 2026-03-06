using UnityEngine;

namespace DemoModeA
{
    [CreateAssetMenu(fileName = "EmotionProfile", menuName = "DemoModeA/EmotionProfile")]
    public class EmotionProfile : ScriptableObject
    {
        public EmotionType Emotion;
        [Range(0.5f, 2f)] public float VoiceRate = 1f;
        [Range(0f, 1f)] public float VoiceVolume = 0.7f;
        public TextLengthBias TextLengthBias = TextLengthBias.Medium;
        public AffirmationBias AffirmationBias = AffirmationBias.Neutral;
        [Range(0f, 1f)] public float EmojiDensity = 0.1f;
        public string[] EmojiPool = new string[] { "🙂" };
        public int PoseId = 0;
        [Range(0f, 1f)] public float LayerWeight = 1f;
        [Range(0.5f, 2f)] public float AnimSpeed = 1f;
        [Range(0f, 1f)] public float ExpressIntensity = 0.5f;
        public AttentionMode AttentionMode = AttentionMode.Focused;
        public float TypingSpeed = 12f;
        public float ResponseDelay = 0.2f;
        public PunctuationStyle PunctuationStyle = PunctuationStyle.Neutral;
        public VisualMoodPreset VisualPreset = VisualMoodPreset.Neutral;
        [Range(0f, 1f)] public float VolumeBlendWeight = 0.5f;

        private void OnValidate()
        {
            VoiceRate = Mathf.Clamp(VoiceRate, 0.5f, 2f);
            VoiceVolume = Mathf.Clamp01(VoiceVolume);
            EmojiDensity = Mathf.Clamp01(EmojiDensity);
            LayerWeight = Mathf.Clamp01(LayerWeight);
            AnimSpeed = Mathf.Clamp(AnimSpeed, 0.5f, 2f);
            ExpressIntensity = Mathf.Clamp01(ExpressIntensity);
            VolumeBlendWeight = Mathf.Clamp01(VolumeBlendWeight);
            if (EmojiPool == null || EmojiPool.Length == 0)
            {
                EmojiPool = new[] { "🙂" };
            }
        }
    }
}
