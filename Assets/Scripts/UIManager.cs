using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private TextMeshProUGUI _gameOverText;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    private int _score = 0;

    public void Initialize()
    {
        // 可以在这里进行初始化操作
        _score = 0;
        _scoreText.text = $"Score: {_score}";

        _gameOverText.text = "Game Over!";
    }

    public void SetScore(int score)
    {
        _score = score;
        UpdateScoreDisplay();
    }

    public void UpdateScoreDisplay()
    {
        Debug.Log($"Score: {_score}");

        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {_score}";
        }
    }

    public void ShowStartButton(bool show)
    {
        if (_startButton != null)
        {
            _startButton.gameObject.SetActive(show);
        }
    }

    public void ShowGameOverText(bool show, int finalScore)
    {
        if (_gameOverText != null)
        {
            if (show)
            {
                _gameOverText.text = "Game Over!\nScore: " + finalScore + "\nClick Start to play again.";
            }
            _gameOverText.gameObject.SetActive(show);
        }
    }

    public Button GetStartButton()
    {
        return _startButton;
    }
}