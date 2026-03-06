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
                _animator = GetComponent<Animator>();
            }
            if (_animator == null || profile == null) return;
            _animator.SetFloat(_animSpeedParam, profile.AnimSpeed);
            _animator.SetInteger(_poseIdParam, profile.PoseId);
        }
    }
}
