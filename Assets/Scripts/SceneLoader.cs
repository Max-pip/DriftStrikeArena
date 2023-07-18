using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonClickClip;

    private string _nameMainMenuScene = "MainMenuScene";
    private string _nameSelectPlayerScene = "SelectPlayerScene";
    private string _namePlayScene = "PlayScene";

    public void Restart()
    {
        AudioManager.Instance.ClickButtonSound(_buttonClickClip);
        string currentName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentName);
    }

    public void MainMenuScene()
    {
        AudioManager.Instance.ClickButtonSound(_buttonClickClip);
        SceneManager.LoadScene(_nameMainMenuScene);
    }

    public void SelectPlayerScene()
    {
        AudioManager.Instance.ClickButtonSound(_buttonClickClip);
        SceneManager.LoadScene(_nameSelectPlayerScene);
    }

    public void PlayScene()
    {
        AudioManager.Instance.ClickButtonSound(_buttonClickClip);
        SceneManager.LoadScene(_namePlayScene);
    }
}
