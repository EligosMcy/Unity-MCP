using UnityEngine;

namespace DemoModeA
{
    public class CharacterPresenter : MonoBehaviour, IEmotionChangeListener, ICharacterPresenter
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _animSpeedParam = "AnimSpeed";
        [SerializeField] private string _poseIdParam = "PoseId";

        public void OnEmotionChanged(EmotionType oldEmotion, EmotionType newEmotion, EmotionProfile profile)
        {
            ApplyEmotion(profile);
        }

        public void ApplyEmotion(EmotionProfile profile)
        {
            if (_animator == null)
            {
                Debug.LogError($"[{nameof(CharacterPresenter)}] Animator is null. Please assign Animator in inspector.", this);
                return;
            }
            if (profile == null) return;
            _animator.SetFloat(_animSpeedParam, profile.AnimSpeed);
            _animator.SetInteger(_poseIdParam, profile.PoseId);
        }
    }
}
