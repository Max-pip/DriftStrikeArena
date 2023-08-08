using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string NamePlayScene = "PlayScene";
    private const string NamePlayScene2 = "PlayScene2";
    private const string NamePlayScene3 = "PlayScene3";
    private const string NamePlayScene4 = "PlayScene4";
    private const string NameTestPlayScene = "TestPlayScene";

    private string _currentSceneName;
    private bool _isBattleScene = false;

    [Header("Audio source")]
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioSource _musicSource;

    [Header("Scene clips")]
    [SerializeField] private AudioClip _startClip;
    [SerializeField] private AudioClip _playClip;

    [Header("Car clips")]
    public AudioClip accelerationClip;
    public AudioClip breakingClip;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        _currentSceneName = scene.name;
        if (_currentSceneName == NamePlayScene || _currentSceneName == NameTestPlayScene || _currentSceneName == NamePlayScene2 || _currentSceneName == NamePlayScene3 || _currentSceneName == NamePlayScene4)
        {
            if (!_isBattleScene)
            {
                PlayBackMusic(_playClip);
            }
            _isBattleScene = true;
        } else
        {
            _isBattleScene = false;
            if (_musicSource.clip == _startClip)
            {
                return;
            } else
            {
                PlayBackMusic(_startClip);
            }
        }
    }

    public void ClickButtonSound(AudioClip audioClip)
    {
        _soundSource.PlayOneShot(audioClip);
    }

    #region SessionAudioMethods

    public void PlayBackMusic(AudioClip audioClip)
    {
        _musicSource.clip = audioClip;
        _musicSource.Play();
        if (MainManager.Instance.isMusicOn)
        {
            _musicSource.mute = false;
        } else
        {
            _musicSource.mute = true;
        }

        if (MainManager.Instance.isSoundOn)
        {
            _soundSource.mute = false;
        }
        else
        {
            _soundSource.mute = true;
        }
    }

    public void MusicOn()
    {
        MainManager.Instance.isMusicOn = true;
        _musicSource.mute = false;
        MainManager.Instance.SaveGameData();
    }

    public void MusicOff()
    {
        MainManager.Instance.isMusicOn = false;
        _musicSource.mute = true;
        MainManager.Instance.SaveGameData();
    }

    public void SoundOn()
    {
        MainManager.Instance.isSoundOn = true;
        _soundSource.mute = false;
        MainManager.Instance.SaveGameData();
    }

    public void SoundOff()
    {
        MainManager.Instance.isSoundOn = false;
        _soundSource.mute = true;
        MainManager.Instance.SaveGameData();
    }

    #endregion

    #region ForSelectModelScene
    public void PurchaseSound(AudioClip audioClip)
    {
        _soundSource.PlayOneShot(audioClip);
    }

    public void SelectSound(AudioClip audioClip)
    {
        _soundSource.PlayOneShot(audioClip);
    }

    public void SwapSound(AudioClip audioClip)
    {
        _soundSource.PlayOneShot(audioClip);
    }

    #endregion

    #region ForPlayScene

    public void WinSound(AudioClip audioClip)
    {
        _soundSource.PlayOneShot(audioClip);
    }

    public void LoseSound(AudioClip audioClip)
    {
        _soundSource.PlayOneShot(audioClip);
    }

    #endregion
}
