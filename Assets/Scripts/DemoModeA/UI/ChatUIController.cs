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
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private TextMeshProUGUI _chatLogText;

        private IDialogueGenerator _dialogue;
        private IEmotionController _emotionController;
        private IEmotionProfileRepository _profiles;

        private void Awake()
        {
            _dialogue = _dialogueSource as IDialogueGenerator;
            _emotionController = _emotionControllerSource as IEmotionController;
            _profiles = _profileRepositorySource as IEmotionProfileRepository;

            if (_dialogue == null) Debug.LogError($"[{nameof(ChatUIController)}] DialogueSource is null or invalid.", this);
            if (_emotionController == null) Debug.LogError($"[{nameof(ChatUIController)}] EmotionControllerSource is null or invalid.", this);
            if (_profiles == null) Debug.LogError($"[{nameof(ChatUIController)}] ProfileRepositorySource is null or invalid.", this);
            if (_inputField == null) Debug.LogError($"[{nameof(ChatUIController)}] InputField is null. Please assign in inspector.", this);
            if (_sendButton == null) Debug.LogError($"[{nameof(ChatUIController)}] SendButton is null. Please assign in inspector.", this);
            if (_chatLogText == null) Debug.LogError($"[{nameof(ChatUIController)}] ChatLogText is null. Please assign in inspector.", this);
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
            var profile = _profiles.GetProfile(_emotionController.CurrentEmotion);
            appendLine($"User: {text}");
            var reply = await _dialogue.GenerateReplyAsync(text, profile);
            appendLine($"Reply: {reply}");
            Debug.Log($"Reply: {reply}");
        }

        private void appendLine(string line)
        {
            if (_chatLogText == null) return;
            if (string.IsNullOrEmpty(_chatLogText.text))
            {
                _chatLogText.text = line;
            }
            else
            {
                _chatLogText.text += "\n" + line;
            }
            Canvas.ForceUpdateCanvases();
            if (_scrollRect != null)
            {
                _scrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }
}
