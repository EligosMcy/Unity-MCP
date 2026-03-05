using UnityEngine;
using UnityEngine.UI;

namespace DemoModeA
{
    public class EmotionUIButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private EmotionType _emotion;
        [SerializeField] private MonoBehaviour _controllerSource;
        private IEmotionController _controller;

        private void Awake()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
            _controller = _controllerSource as IEmotionController;
            if (_controller == null)
            {
                _controller = FindFirstObjectByType<EmotionController>();
            }
            if (_button != null)
            {
                _button.onClick.AddListener(onClick);
            }
        }

        private void onClick()
        {
            if (_controller == null) return;
            _controller.SetEmotion(_emotion);
        }
    }
}
