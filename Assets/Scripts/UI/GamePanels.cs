using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanels : MonoBehaviour
{
    [SerializeField] private Canvas _UIHud;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private TextMeshProUGUI _winBalance;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private TextMeshProUGUI _loseBalance;
    [SerializeField] private GameObject _playPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;

    private void Start()
    {
        Time.timeScale = 1f;

        _pauseButton.onClick.AddListener(delegate
        {
            onClickedPauseButton();
        });

        _resumeButton.onClick.AddListener(delegate 
        { 
            onClickedResumeButton();
        });
    }

    private void OnEnable()
    {
        CarControllerAI.onWinPanel += WinPanel;
        DeadZoneTrigger.onLosePanel += LosePanel;
    }

    private void OnDisable()
    {
        CarControllerAI.onWinPanel -= WinPanel;
        DeadZoneTrigger.onLosePanel -= LosePanel;
    }

    private void onClickedPauseButton()
    {
        GameManager.Instance.isPaused = true;
        Time.timeScale = 0;
        _UIHud.enabled = false;
        _playPanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    private void onClickedResumeButton()
    {
        GameManager.Instance.isPaused = false;
        Time.timeScale = 1;
        _UIHud.enabled = true;
        _pausePanel.SetActive(false);
        _playPanel.SetActive(true);
    }

    private void WinPanel()
    {
        GameManager.Instance.isGameOver = true;
        _UIHud.enabled = false;
        _playPanel.SetActive(false);
        _winPanel.SetActive(true);
        MainManager.Instance.coins += 1;
        MainManager.Instance.SaveGameData();
        _winBalance.text = $"Your balance: {MainManager.Instance.coins}";
    }

    private void LosePanel()
    {
        GameManager.Instance.isGameOver = true;
        _UIHud.enabled = false;
        _playPanel.SetActive(false);
        _losePanel.SetActive(true);
        _loseBalance.text = $"Your balance: {MainManager.Instance.coins}";
    }
}
