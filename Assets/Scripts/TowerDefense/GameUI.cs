using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private int _initialGold = 1;
        
        private TextMeshProUGUI _goldText;
        private TextMeshProUGUI _monsterCountText;
        private Button _buildButton;
        private TextMeshProUGUI _buildCostText;
        private Button _pauseButton;
        private TextMeshProUGUI _pauseButtonText;

        private Canvas _canvas;
        private bool _isPaused = false;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            if (_canvas == null)
            {
                _canvas = gameObject.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            CreateUI();
        }

        private void Start()
        {
            if (_buildButton != null)
            {
                _buildButton.onClick.AddListener(OnBuildButtonClicked);
            }

            if (_pauseButton != null)
            {
                _pauseButton.onClick.AddListener(OnPauseButtonClicked);
            }

            if (TowerDefenseGameManager.Instance != null)
            {
                TowerDefenseGameManager.Instance._onGoldChanged.AddListener(OnGoldChanged);
            }
        }

        private void CreateUI()
        {
            GameObject topPanel = CreatePanel("TopPanel", transform);
            RectTransform topRect = topPanel.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0, 1);
            topRect.anchorMax = new Vector2(1, 1);
            topRect.pivot = new Vector2(0.5f, 1);
            topRect.offsetMin = new Vector2(20, -20);
            topRect.offsetMax = new Vector2(-20, -60);
            topPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

            GameObject goldObj = CreateText("GoldText", topPanel.transform, $"Gold: {_initialGold}", 36);
            _goldText = goldObj.GetComponent<TextMeshProUGUI>();
            RectTransform goldRect = goldObj.GetComponent<RectTransform>();
            goldRect.anchorMin = new Vector2(0, 0.5f);
            goldRect.anchorMax = new Vector2(0, 0.5f);
            goldRect.pivot = new Vector2(0, 0.5f);
            goldRect.anchoredPosition = new Vector2(20, 0);

            GameObject monsterObj = CreateText("MonsterCountText", topPanel.transform, "Monsters: 0", 36);
            _monsterCountText = monsterObj.GetComponent<TextMeshProUGUI>();
            RectTransform monsterRect = monsterObj.GetComponent<RectTransform>();
            monsterRect.anchorMin = new Vector2(1, 0.5f);
            monsterRect.anchorMax = new Vector2(1, 0.5f);
            monsterRect.pivot = new Vector2(1, 0.5f);
            monsterRect.anchoredPosition = new Vector2(-20, 0);

            GameObject bottomPanel = CreatePanel("BottomPanel", transform);
            RectTransform bottomRect = bottomPanel.GetComponent<RectTransform>();
            bottomRect.anchorMin = new Vector2(0, 0);
            bottomRect.anchorMax = new Vector2(1, 0);
            bottomRect.pivot = new Vector2(0.5f, 0);
            bottomRect.offsetMin = new Vector2(20, 20);
            bottomRect.offsetMax = new Vector2(-20, 100);
            bottomPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

            GameObject buildBtnObj = CreateButton("BuildButton", bottomPanel.transform, "Build Floor");
            _buildButton = buildBtnObj.GetComponent<Button>();
            RectTransform buildBtnRect = buildBtnObj.GetComponent<RectTransform>();
            buildBtnRect.anchorMin = new Vector2(0, 0.5f);
            buildBtnRect.anchorMax = new Vector2(0, 0.5f);
            buildBtnRect.pivot = new Vector2(0, 0.5f);
            buildBtnRect.anchoredPosition = new Vector2(20, 0);
            buildBtnRect.sizeDelta = new Vector2(200, 60);

            Transform buildTextParent = buildBtnObj.transform.GetChild(0);
            _buildCostText = buildTextParent.GetComponent<TextMeshProUGUI>();

            GameObject pauseBtnObj = CreateButton("PauseButton", bottomPanel.transform, "Pause");
            _pauseButton = pauseBtnObj.GetComponent<Button>();
            RectTransform pauseBtnRect = pauseBtnObj.GetComponent<RectTransform>();
            pauseBtnRect.anchorMin = new Vector2(1, 0.5f);
            pauseBtnRect.anchorMax = new Vector2(1, 0.5f);
            pauseBtnRect.pivot = new Vector2(1, 0.5f);
            pauseBtnRect.anchoredPosition = new Vector2(-20, 0);
            pauseBtnRect.sizeDelta = new Vector2(120, 60);

            Transform pauseTextParent = pauseBtnObj.transform.GetChild(0);
            _pauseButtonText = pauseTextParent.GetComponent<TextMeshProUGUI>();
        }

        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
            return panel;
        }

        private GameObject CreateText(string name, Transform parent, string text, int fontSize)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            RectTransform rect = textObj.AddComponent<RectTransform>();
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            return textObj;
        }

        private GameObject CreateButton(string name, Transform parent, string text)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);
            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            Image image = buttonObj.AddComponent<Image>();
            image.color = Color.gray;
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.highlightedColor = Color.white;
            colors.pressedColor = Color.darkGray;
            button.colors = colors;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return buttonObj;
        }

        private void OnGoldChanged(int gold)
        {
            UpdateGold(gold);
        }

        public void UpdateGold(int gold)
        {
            if (_goldText != null)
            {
                _goldText.text = $"Gold: {gold}";
            }
            UpdateBuildButton();
        }

        public void UpdateMonsterCount(int count)
        {
            if (_monsterCountText != null)
            {
                _monsterCountText.text = $"Monsters: {count}";
            }
        }

        private void UpdateBuildButton()
        {
            if (TowerDefenseGameManager.Instance != null)
            {
                int cost = TowerDefenseGameManager.Instance.GetNextFloorCost();
                int currentGold = TowerDefenseGameManager.Instance.Gold;

                if (_buildCostText != null)
                {
                    _buildCostText.text = $"Build Floor ({cost} Gold)";
                }

                if (_buildButton != null)
                {
                    _buildButton.interactable = currentGold >= cost;
                }
            }
        }

        private void OnBuildButtonClicked()
        {
            if (TowerDefenseGameManager.Instance != null)
            {
                TowerDefenseGameManager.Instance.BuildNewFloor();
            }
        }

        private void OnPauseButtonClicked()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0 : 1;

            if (_pauseButtonText != null)
            {
                _pauseButtonText.text = _isPaused ? "Resume" : "Pause";
            }
        }
    }
}
