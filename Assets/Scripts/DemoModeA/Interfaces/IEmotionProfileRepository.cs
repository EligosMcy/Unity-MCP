namespace DemoModeA
{
    public interface IEmotionProfileRepository
    {
        EmotionProfile GetProfile(EmotionType emotion);
    }
}
