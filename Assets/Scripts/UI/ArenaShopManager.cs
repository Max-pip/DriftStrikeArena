
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.Scripts;
using System.IO;

public class ArenaShopManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _nextArenaModelButton;
    [SerializeField] private Button _previousArenaModelButton;
    [SerializeField] private Button _purchaseButton;
    [SerializeField] private Button _selectButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private TextMeshProUGUI _selectText;
    [SerializeField] private TextMeshProUGUI _priceText;

    [Header("Sounds")]
    [SerializeField] private AudioClip _purchaseClip;
    [SerializeField] private AudioClip _cannotPurchaseClip;
    [SerializeField] private AudioClip _selectClip;
    [SerializeField] private AudioClip _swapModelClip;

    [Header("Arenas data")]
    [SerializeField] private ArenaModelsData[] _arenaDataModels;

    private GameObject _purchaseButtonObj;
    private GameObject _selectButtonObj;

    private Color _cannotPurchaseColor = Color.red;
    private float _durationChangeColorText = 0.25f;

    private int _currentModelIndex = 0;
    private GameObject _currentModel;

    private const string ArenaPrefsKey = "PurchasedArena";

    private void Start()
    {
        AllDelegateButtonAction();

        _purchaseButtonObj = GameObject.FindGameObjectWithTag("PurchaseButton");
        _selectButtonObj = GameObject.FindGameObjectWithTag("SelectButton");

        ArenaModelsData currentArenaModel = _arenaDataModels[_currentModelIndex];
        if (currentArenaModel.isPurchased)
        {
            _priceText.gameObject.SetActive(false);
            _purchaseButtonObj.SetActive(false);
            _selectButtonObj.SetActive(true);
        }
        else
        {
            _priceText.gameObject.SetActive(true);
            _priceText.text = $"Price: {currentArenaModel.price}";
            _purchaseButtonObj.SetActive(true);
            _selectButtonObj.SetActive(false);
        }

        MainManager.Instance.LoadGameData();
        LoadSelectedArenaModelIndex();
        LoadPurchasedPlayerModel();
        LoadIsSelectedArenaModel();
        SpawnModel();
        SelectArenaModel();
        _balanceText.text = $"Balance: {MainManager.Instance.coins}";
    }

    private void AllDelegateButtonAction()
    {
        _nextArenaModelButton.onClick.AddListener(delegate
        {
            NextArenaModel();
        });

        _previousArenaModelButton.onClick.AddListener(delegate
        {
            PreviousArenaModel();
        });

        _purchaseButton.onClick.AddListener(delegate
        {
            PurchaseArenaModel();
        });

        _selectButton.onClick.AddListener(delegate
        {
            SelectArenaModel();
        });
    }

    public void NextArenaModel()
    {
        AudioManager.Instance.SwapSound(_swapModelClip);
        _currentModelIndex++;
        if (_currentModelIndex >= _arenaDataModels.Length)
        {
            _currentModelIndex = 0;
        }

        ArenaModelsData currentArenaModel = _arenaDataModels[_currentModelIndex];
        if (currentArenaModel.isPurchased)
        {
            _priceText.gameObject.SetActive(false);
            _purchaseButtonObj.SetActive(false);
            _selectButtonObj.SetActive(true);
        }
        else
        {
            _priceText.gameObject.SetActive(true);
            _priceText.text = $"Price: {currentArenaModel.price}";
            _purchaseButtonObj.SetActive(true);
            _selectButtonObj.SetActive(false);
        }

        SpawnModel();
    }

    public void PreviousArenaModel()
    {
        AudioManager.Instance.SwapSound(_swapModelClip);
        _currentModelIndex--;
        if (_currentModelIndex < 0)
        {
            _currentModelIndex = _arenaDataModels.Length - 1;
        }

        ArenaModelsData currentArenaModel = _arenaDataModels[_currentModelIndex];
        if (currentArenaModel.isPurchased)
        {
            _priceText.gameObject.SetActive(false);
            _purchaseButtonObj.SetActive(false);
            _selectButtonObj.SetActive(true);
        }
        else
        {
            _priceText.gameObject.SetActive(true);
            _priceText.text = $"Price: {currentArenaModel.price}";
            _purchaseButtonObj.SetActive(true);
            _selectButtonObj.SetActive(false);
        }

        SpawnModel();
    }

    private void SpawnModel()
    {
        if (_currentModel != null)
        {
            Destroy(_currentModel);
        }

        _currentModel = Instantiate(_arenaDataModels[_currentModelIndex].model);

        if (_arenaDataModels[_currentModelIndex].isSelected)
        {
            _selectText.text = "Selected";
        }
        else
        {
            _selectText.text = "Select";
        }
    }

    public void SelectArenaModel()
    {
        ArenaModelsData selectedPlayerModel = _arenaDataModels[_currentModelIndex];
        if (selectedPlayerModel.isPurchased)
        {
            AudioManager.Instance.SelectSound(_selectClip);
            selectedPlayerModel.isSelected = true;

            for (int i = 0; i < _arenaDataModels.Length; i++)
            {
                if (i != _currentModelIndex)
                {
                    _arenaDataModels[i].isSelected = false;
                }
            }

            _selectText.text = "Selected";

            MainManager.Instance.ArenaName = _arenaDataModels[_currentModelIndex].arenaName;
            MainManager.Instance.selectedArenaModelIndex = _currentModelIndex;
            SaveIsSelectedArenaModel();
            MainManager.Instance.SaveGameData();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }

    public void PurchaseArenaModel()
    {
        ArenaModelsData desiredPurchaseArenaModel = _arenaDataModels[_currentModelIndex];

        if (MainManager.Instance.coins >= desiredPurchaseArenaModel.price)
        {
            AudioManager.Instance.PurchaseSound(_purchaseClip);
            MainManager.Instance.coins -= desiredPurchaseArenaModel.price;
            desiredPurchaseArenaModel.isPurchased = true;

            _priceText.gameObject.SetActive(false);
            _balanceText.text = $"Balance: {MainManager.Instance.coins}";

            SavePurchasedArenaModel();
            MainManager.Instance.SaveGameData();

            _selectButtonObj?.SetActive(true);
            _purchaseButtonObj?.SetActive(false);
        }
        else
        {
            ChangeTextColor();
            AudioManager.Instance.PurchaseSound(_cannotPurchaseClip);
        }
    }

    private void ChangeTextColor()
    {
        _balanceText.DOColor(_cannotPurchaseColor, _durationChangeColorText).OnComplete(ChangeTextColorBack);
    }

    private void ChangeTextColorBack()
    {
        _balanceText.DOColor(Color.white, _durationChangeColorText);
    }

    
    #region SaveLoadMethods

    private void SavePurchasedArenaModel()
    {
        List<int> purchasedIndices = new List<int>();
        for (int i = 0; i < _arenaDataModels.Length; i++)
        {
            if (_arenaDataModels[i].isPurchased)
            {
                purchasedIndices.Add(i);
            }
        }
        string Arena = string.Join(",", purchasedIndices);
        PlayerPrefs.SetString(ArenaPrefsKey, Arena);
    }

    private void LoadPurchasedPlayerModel()
    {
        if (PlayerPrefs.HasKey(ArenaPrefsKey))
        {
            string purchasedArenaModel = PlayerPrefs.GetString(ArenaPrefsKey);
            string[] purchasedArenaModelIndices = purchasedArenaModel.Split(',');
            for (int i = 0; i < purchasedArenaModelIndices.Length; i++)
            {
                int arenaModelIndex;
                if (int.TryParse(purchasedArenaModelIndices[i], out arenaModelIndex))
                {
                    if (arenaModelIndex >= 0 && arenaModelIndex < _arenaDataModels.Length)
                    {
                        _arenaDataModels[arenaModelIndex].isPurchased = true;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid arena model index: " + arenaModelIndex);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to parse arena model index: " + purchasedArenaModelIndices[i]);
                }
            }
        }
    }

    public void SaveIsSelectedArenaModel()
    {
        PlayerPrefs.SetInt("CurrentArenaModelIndex", _currentModelIndex);

        for (int i = 0; i < _arenaDataModels.Length; i++)
        {
            string key = $"ArenaModel{i}_IsSelected";
            int isSelectedValue = _arenaDataModels[i].isSelected ? 1 : 0;
            PlayerPrefs.SetInt(key, isSelectedValue);
        }

        PlayerPrefs.Save();
    }

    public void LoadIsSelectedArenaModel()
    {
        _currentModelIndex = PlayerPrefs.GetInt("CurrentArenaModelIndex");

        for (int i = 0; i < _arenaDataModels.Length; i++)
        {
            string key = $"ArenaModel{i}_IsSelected";
            int isSelectedValue = PlayerPrefs.GetInt(key);
            _arenaDataModels[i].isSelected = isSelectedValue == 1;
        }
    }

    private void LoadSelectedArenaModelIndex()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            _currentModelIndex = data.SelectedArenaModelIndex;
        }
    }
    #endregion
    
}
