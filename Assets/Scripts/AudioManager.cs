using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _audioSource;

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

    public void ClickButtonSound(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }

    #region ForSelectModelScene
    public void PurchaseSound(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }

    public void SelectSound(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }

    public void SwapSound(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }

    #endregion
}
