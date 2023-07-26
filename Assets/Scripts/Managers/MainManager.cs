using Assets.Scripts;
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

    private string _arenaName = "PlayScene";

    public string ArenaName
    {
        get { return _arenaName; }
        set { _arenaName = value; }
    }

    public int coins = 0;
    public int selectedModelIndex = 0;

    public int selectedArenaModelIndex = 0;

    public bool isMusicOn = true;
    public bool isSoundOn = true;

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
        data.SelectedArenaModelIndex = selectedArenaModelIndex;
        data.IsMusicOn = isMusicOn;
        data.IsSoundOn = isSoundOn;

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
            selectedArenaModelIndex = data.SelectedArenaModelIndex;
            isMusicOn = data.IsMusicOn;
            isSoundOn = data.IsSoundOn;
        }
    }
}
