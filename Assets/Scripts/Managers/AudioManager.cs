using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string NamePlayScene = "PlayScene";
    private const string NameTestPlayScene = "TestPlayScene";

    private string _currentSceneName;

    [Header("Audio source")]
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioSource _musicSource;

    [Header("Audio clips")]
    [SerializeField] private AudioClip _startClip;
    [SerializeField] private AudioClip _playClip;

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
        if (_currentSceneName == NamePlayScene || _currentSceneName == NameTestPlayScene)
        {
            PlayBackMusic(_playClip);
        } else
        {
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
