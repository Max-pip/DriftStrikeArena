using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; set; }

    private GameObject _playerPrefab;

    public GameObject PlayerPrefab 
    { 
        get { return _playerPrefab; } 
        set { _playerPrefab = value; }
    }

    public int coins = 0;
    public int selectedModelIndex = 0;

    private void Awake()
    {
        LoadGameData();

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    public void SaveGameData()
    {
        SaveData data = new SaveData();
        data.CoinBalance = coins;
        data.SelectedPlayerModelIndex = selectedModelIndex;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGameData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            coins = data.CoinBalance;
            selectedModelIndex = data.SelectedPlayerModelIndex;
        }
    }
}
