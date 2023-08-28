using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class GamePanels : MonoBehaviour
{
    private AdManager _adManager;
    
    private const string HudTag = "Hud";
    [SerializeField] private Canvas _UIHud;

    [Header("Panels")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _playPanel;
    [SerializeField] private GameObject _pausePanel;
    [Header("Text")]
    [SerializeField] private GameObject _startTextContainer;
    [SerializeField] private TextMeshProUGUI _winBalance;
    [SerializeField] private TextMeshProUGUI _trollComment;
    [SerializeField] private TextMeshProUGUI _loseBalance;
    [SerializeField] private TextMeshProUGUI _startText;
    private List<string> _trollCommentsList = new List<string>() 
    {
        "I hope you found the ad useful)",
        "How do you like our advertising?",
        "If you lose, watch the ad",
        "you won an advertisement)",
        "After such advertising, it is not a shame to lose)",
        "Maybe you just like to watch ads?",
        "You will succeed, but in the meantime, watch the ads(",
        "We are very interested in what they showed you, can you tell us?",
        "I hope this ad has broadened your horizons",
        "You can lose for the sake of such advertising",
        "advertising, advertising, advertising...",
        "Oh, no! You've lost again (hee hee hee)",
        "We are truly sorry (sarcasm)",
        "Your punishment is 10 push-ups and an advertisement",
        "You probably lost because of a malicious developer",
        "Patience and perseverance conquer all, but so far, advertising",
        "I'm sure you like advertising)"
    };
    [Header("Buttons")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [Header("Audio clips")]
    [SerializeField] private AudioClip _winClip;
    [SerializeField] private AudioClip _loseClip;
    [SerializeField] private AudioClip _clickClip;

    private float _initialSizeFont;
    private float _initialSizeFontBalance;
    private float _targetMaxSizeFontBalance = 85f;
    private float _targetMaxSizeFont = 110f;
    private float _targetMinSizeFont = 2f;
    private float _increaseDuration = 0.8f;
    private float _decreaseDuration = 0.35f;
    private float _fadeOutDuration = 0.35f;

    private void Start()
    {
        Time.timeScale = 1f;

        _adManager = GameObject.FindAnyObjectByType<AdManager>();
        _UIHud = GameObject.FindGameObjectWithTag(HudTag)?.GetComponent<Canvas>();

        _initialSizeFont = _startText.fontSize;

        _pauseButton.onClick.AddListener(delegate
        {
            onClickedPauseButton();
        });

        _resumeButton.onClick.AddListener(delegate 
        { 
            onClickedResumeButton();
        });

        _adManager.LoadInterstitial();

        StartCoroutine(AnimateTextSizeCoroutine());
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
        AudioManager.Instance.ClickButtonSound(_clickClip);
        Time.timeScale = 0;
        if (_startTextContainer != null )
        {
            _startTextContainer.SetActive(false);
        }
        _UIHud.enabled = false;
        _playPanel.SetActive(false);
        _pausePanel.SetActive(true);
    }

    private void onClickedResumeButton()
    {
        GameManager.Instance.isPaused = false;
        AudioManager.Instance.ClickButtonSound(_clickClip);
        Time.timeScale = 1;
        _UIHud.enabled = true;
        _pausePanel.SetActive(false);
        _playPanel.SetActive(true);
    }

    private void WinPanel()
    {
        GameManager.Instance.isGameOver = true;
        AudioManager.Instance.WinSound(_winClip);
        _UIHud.enabled = false;
        _playPanel.SetActive(false);
        _winPanel.SetActive(true);
        MainManager.Instance.coins += 1;
        MainManager.Instance.SaveGameData();
        _winBalance.text = $"Your balance: {MainManager.Instance.coins}";
        StartCoroutine(AnimateBalanceTextCoroutine(_winBalance));
    }

    private void LosePanel()
    {
        GameManager.Instance.isGameOver = true;
        AudioManager.Instance.LoseSound(_loseClip);
        _UIHud.enabled = false;
        _playPanel.SetActive(false);
        _losePanel.SetActive(true);
        _adManager.ShowInterstitial();
        _trollComment.text = _trollCommentsList[Random.Range(0, _trollCommentsList.Count)];
        _loseBalance.text = $"Your balance: {MainManager.Instance.coins}";
        StartCoroutine(AnimateBalanceTextCoroutine(_loseBalance));
    }

    private IEnumerator AnimateBalanceTextCoroutine(TextMeshProUGUI balanceText)
    {
        _initialSizeFontBalance = balanceText.fontSize;

        yield return new WaitForSeconds(0.3f);

        float timePassed = 0f;

        while (timePassed < _increaseDuration)
        {
            timePassed += Time.deltaTime;
            float t = Mathf.Clamp01(timePassed / _increaseDuration);
            float newSize = Mathf.Lerp(_initialSizeFontBalance, _targetMaxSizeFontBalance, t);
            balanceText.fontSize = newSize;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        timePassed = 0f;

        while (timePassed < _decreaseDuration)
        {
            timePassed += Time.deltaTime;
            float t = Mathf.Clamp01(timePassed / _decreaseDuration);
            float newSize = Mathf.Lerp(_targetMaxSizeFontBalance, _initialSizeFontBalance, t);
            balanceText.fontSize = newSize;
            yield return null;
        }
    }

    private IEnumerator AnimateTextSizeCoroutine()
    {
        float timePassed = 0f;

        while (timePassed < _increaseDuration)
        {
            timePassed += Time.deltaTime;
            float t = Mathf.Clamp01(timePassed / _increaseDuration);
            float newSize = Mathf.Lerp(_initialSizeFont, _targetMaxSizeFont, t);
            _startText.fontSize = newSize;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        timePassed = 0f;

        while (timePassed < _decreaseDuration)
        {
            timePassed += Time.deltaTime;
            float t = Mathf.Clamp01(timePassed / _decreaseDuration);
            float newSize = Mathf.Lerp(_targetMaxSizeFont, _targetMinSizeFont, t);
            _startText.DOFade(0f, _fadeOutDuration);
            _startText.fontSize = newSize;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        Destroy(_startTextContainer);
    }
}
