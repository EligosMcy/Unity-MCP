using System;

namespace DemoModeA
{
    public interface IEmotionController
    {
        EmotionType CurrentEmotion { get; }
        DateTime? ExpireAt { get; }
        event Action<EmotionType, EmotionType> OnEmotionChanged;
        void SetEmotion(EmotionType emotion);
        void ResetEmotion();
        float GetRemainingTime();
    }
}
