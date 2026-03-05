using UnityEngine;

namespace DemoModeA
{
    [CreateAssetMenu(fileName = "EmotionProfileSet", menuName = "DemoModeA/EmotionProfileSet")]
    public class EmotionProfileSet : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public EmotionType Emotion;
            public EmotionProfile Profile;
        }

        [SerializeField] public Entry[] Entries = new Entry[0];
    }
}
