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
        [SerializeField] private bool _enableLogs = true;
        [Space]
        [SerializeField] private MonoBehaviour _controllerSource;
        private IEmotionController _controller;

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(onClick);
            }
            else if (_enableLogs)
            {
                Debug.LogWarning($"[{nameof(EmotionUIButton)}] Button reference is null. Click won't be handled.", this);
            }

            _controller = _controllerSource as IEmotionController ?? FindFirstObjectByType<EmotionController>();
            if (_enableLogs)
            {
                Debug.Log($"[{nameof(EmotionUIButton)}] Awake. Emotion={_emotion}, Controller={(_controller != null ? _controller.GetType().Name : "null")}", this);
                if (_controller == null)
                {
                    Debug.LogWarning($"[{nameof(EmotionUIButton)}] No IEmotionController found. Click won't change emotion.", this);
                }
            }

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
            if (_enableLogs)
            {
                Debug.Log($"[{nameof(EmotionUIButton)}] Clicked. Request emotion={_emotion}", this);
            }

            if (_controller == null) return;
            _controller.SetEmotion(_emotion);
        }
    }
}
