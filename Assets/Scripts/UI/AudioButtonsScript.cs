using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioButtonsScript : MonoBehaviour
{
    [Header("Audio sprites")]
    [SerializeField] private Sprite _spriteMusicOn;
    [SerializeField] private Sprite _spriteMusicOff;
    [SerializeField] private Sprite _spriteSoundOn;
    [SerializeField] private Sprite _spriteSoundOff;

    [Header("References on images")]
    [SerializeField] private Image _imageMusic;
    [SerializeField] private Image _imageSound;

    [Header("Buttons")]
    [SerializeField] private Button _buttonMusic;
    [SerializeField] private Button _buttonSound;

    private void Start()
    {
        CheckBoolValueMusic();
        CheckBoolValueSound();

        _buttonMusic.onClick.AddListener(delegate
        {
            OnMusicButtonClick();
        });

        _buttonSound.onClick.AddListener(delegate
        {
            OnSoundButtonClick();
        });
    }

    private void OnMusicButtonClick()
    {
        if (MainManager.Instance.isMusicOn)
        {
            AudioManager.Instance.MusicOff();
        } else
        {
            AudioManager.Instance.MusicOn();
        }

        CheckBoolValueMusic();
    }

    private void OnSoundButtonClick()
    {
        if (MainManager.Instance.isSoundOn)
        {
            AudioManager.Instance.SoundOff();
        }
        else
        {
            AudioManager.Instance.SoundOn();
        }

        CheckBoolValueSound();
    }

    private void CheckBoolValueMusic()
    {
        _imageMusic.sprite = MainManager.Instance.isMusicOn ? _spriteMusicOn : _spriteMusicOff;
    }

    private void CheckBoolValueSound()
    {
        _imageSound.sprite = MainManager.Instance.isSoundOn ? _spriteSoundOn : _spriteSoundOff;
    }
}
