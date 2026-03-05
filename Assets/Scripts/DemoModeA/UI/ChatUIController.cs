using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DemoModeA
{
    public class ChatUIController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _sendButton;
        [SerializeField] private MonoBehaviour _dialogueSource;
        [SerializeField] private MonoBehaviour _emotionControllerSource;
        [SerializeField] private MonoBehaviour _profileRepositorySource;

        private IDialogueGenerator _dialogue;
        private IEmotionController _emotionController;
        private IEmotionProfileRepository _profiles;

        private void Awake()
        {
            _dialogue = _dialogueSource as IDialogueGenerator;
            if (_dialogue == null) _dialogue = FindFirstObjectByType<DialogueController>();
            _emotionController = _emotionControllerSource as IEmotionController;
            if (_emotionController == null) _emotionController = FindFirstObjectByType<EmotionController>();
            _profiles = _profileRepositorySource as IEmotionProfileRepository;
            if (_profiles == null) _profiles = FindFirstObjectByType<EmotionProfileRepository>();
            if (_inputField == null) _inputField = GetComponentInChildren<TMPro.TMP_InputField>(true);
            if (_sendButton == null) _sendButton = GetComponentInChildren<Button>(true);
            if (_sendButton != null)
            {
                _sendButton.onClick.AddListener(() => { _ = onSendClicked(); });
            }
        }

        private async Task onSendClicked()
        {
            if (_inputField == null || string.IsNullOrWhiteSpace(_inputField.text)) return;
            var text = _inputField.text;
            _inputField.text = "";
            var profile = _profiles != null ? _profiles.GetProfile(_emotionController?.CurrentEmotion ?? EmotionType.Neutral) : null;
            var reply = _dialogue != null ? await _dialogue.GenerateReplyAsync(text, profile) : "";
            Debug.Log($"User: {text}");
            Debug.Log($"Reply: {reply}");
        }
    }
}
