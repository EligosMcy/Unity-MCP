using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DemoModeA
{
    public class EmotionUIButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private EmotionType _emotion;
        [Space]
        [SerializeField] private MonoBehaviour _controllerSource;
        private IEmotionController _controller;

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(onClick);
            }

            _controller = _controllerSource as IEmotionController ?? FindFirstObjectByType<EmotionController>();

            updateText();
        }

        private void OnValidate()
        {
            updateText();
        }

        private void updateText()
        {
            if (_text != null)
            {
                _text.text = _emotion.ToString();
            }
        }

        private void onClick()
        {
            if (_controller == null) return;
            _controller.SetEmotion(_emotion);
        }
    }
}
