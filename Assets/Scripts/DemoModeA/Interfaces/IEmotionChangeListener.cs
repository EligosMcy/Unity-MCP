namespace DemoModeA
{
    public interface IEmotionChangeListener
    {
        void OnEmotionChanged(EmotionType oldEmotion, EmotionType newEmotion, EmotionProfile profile);
    }
}
