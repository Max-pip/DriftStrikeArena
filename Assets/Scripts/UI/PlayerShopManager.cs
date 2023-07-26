using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerShopManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _nextPlayerModelButton;
    [SerializeField] private Button _previousPlayerModelButton;
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

    private GameObject _purchaseButtonObj;
    private GameObject _selectButtonObj;

    private Color _cannotPurchaseColor = Color.red;
    private float _durationChangeColorText = 0.25f;

    [SerializeField] private PlayerModelsData[] _playerDataModels;

    private int _currentModelIndex = 0;
    private GameObject _currentModel;

    private const string PlayerPrefsKey = "PurchasedCharacters";

    void Start()
    {
        AllDelegateButtonAction();

        _purchaseButtonObj = GameObject.FindGameObjectWithTag("PurchaseButton");
        _selectButtonObj = GameObject.FindGameObjectWithTag("SelectButton");

        PlayerModelsData currentPlayerModel = _playerDataModels[_currentModelIndex];
        if (currentPlayerModel.isPurchased)
        {
            _priceText.gameObject.SetActive(false);
            _purchaseButtonObj.SetActive(false);
            _selectButtonObj.SetActive(true);
        }
        else
        {
            _priceText.gameObject.SetActive(true);
            _priceText.text = $"Price: {currentPlayerModel.price}";
            _purchaseButtonObj.SetActive(true);
            _selectButtonObj.SetActive(false);
        }

        MainManager.Instance.LoadGameData();
        LoadSelectedPlayerModelIndex();
        LoadPurchasedPlayerModel();
        LoadIsSelectedPlayerModel();
        SpawnModel();
        SelectPlayerModel();
        _balanceText.text = $"Balance: {MainManager.Instance.coins}";
    }

    private void AllDelegateButtonAction()
    {
        _nextPlayerModelButton.onClick.AddListener(delegate
        {
            NextPlayerModel();
        });

        _previousPlayerModelButton.onClick.AddListener(delegate
        {
            PreviousPlayerModel();
        });

        _purchaseButton.onClick.AddListener(delegate
        {
            PurchasePlayerModel();
        });

        _selectButton.onClick.AddListener(delegate
        {
            SelectPlayerModel();
        });
    }

    public void NextPlayerModel()
    {
        AudioManager.Instance.SwapSound(_swapModelClip);
        _currentModelIndex++;
        if (_currentModelIndex >= _playerDataModels.Length)
        {
            _currentModelIndex = 0;
        }

        PlayerModelsData currentPlayerModel = _playerDataModels[_currentModelIndex];
        if (currentPlayerModel.isPurchased) 
        {
            _priceText.gameObject.SetActive(false);
            _purchaseButtonObj.SetActive(false);
            _selectButtonObj.SetActive(true);
        } else
        {
            _priceText.gameObject.SetActive(true);
            _priceText.text = $"Price: {currentPlayerModel.price}";
            _purchaseButtonObj.SetActive(true);
            _selectButtonObj.SetActive(false);
        }

        SpawnModel();
    }

    public void PreviousPlayerModel()
    {
        AudioManager.Instance.SwapSound(_swapModelClip);
        _currentModelIndex--;
        if (_currentModelIndex < 0)
        {
            _currentModelIndex = _playerDataModels.Length - 1;
        }

        PlayerModelsData currentPlayerModel = _playerDataModels[_currentModelIndex];
        if (currentPlayerModel.isPurchased)
        {
            _priceText.gameObject.SetActive(false);
            _purchaseButtonObj.SetActive(false);
            _selectButtonObj.SetActive(true);
        }
        else
        {
            _priceText.gameObject.SetActive(true);
            _priceText.text = $"Price: {currentPlayerModel.price}";
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

        _currentModel = Instantiate(_playerDataModels[_currentModelIndex].model, new Vector3(0, 1f, 0), Quaternion.Euler(new Vector3(0, -144f, 0)));

        if (_playerDataModels[_currentModelIndex].isSelected)
        {
            _selectText.text = "Selected";
        } else
        {
            _selectText.text = "Select";
        }
    }

    public void SelectPlayerModel()
    {
        PlayerModelsData selectedPlayerModel = _playerDataModels[_currentModelIndex];
        if (selectedPlayerModel.isPurchased)
        {
            AudioManager.Instance.SelectSound(_selectClip);
            selectedPlayerModel.isSelected = true;

            for (int i = 0; i < _playerDataModels.Length; i++)
            {
                if (i != _currentModelIndex)
                {
                    _playerDataModels[i].isSelected = false;
                }
            }

            _selectText.text = "Selected";

            MainManager.Instance.PlayerPrefab = _playerDataModels[_currentModelIndex].model;
            MainManager.Instance.selectedModelIndex = _currentModelIndex;
            SaveIsSelectedPlayerModel();
            MainManager.Instance.SaveGameData();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }

    public void PurchasePlayerModel()
    {
        PlayerModelsData desiredPurchasePlayerModel = _playerDataModels[_currentModelIndex];

        if (MainManager.Instance.coins >= desiredPurchasePlayerModel.price)
        {
            AudioManager.Instance.PurchaseSound(_purchaseClip);
            MainManager.Instance.coins -= desiredPurchasePlayerModel.price;
            desiredPurchasePlayerModel.isPurchased = true;

            _priceText.gameObject.SetActive(false);
            _balanceText.text = $"Balance: {MainManager.Instance.coins}";

            SavePurchasedPlayerModel();
            MainManager.Instance.SaveGameData();

            _selectButtonObj?.SetActive(true);
            _purchaseButtonObj?.SetActive(false);
        } else
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
    private void SavePurchasedPlayerModel()
    {
        List<int> purchasedIndices = new List<int>();
        for (int i = 0; i < _playerDataModels.Length; i++)
        {
            if (_playerDataModels[i].isPurchased)
            {
                purchasedIndices.Add(i);
            }
        }
        string purchasedPlayerModel = string.Join(",", purchasedIndices);
        PlayerPrefs.SetString(PlayerPrefsKey, purchasedPlayerModel);
    }

    private void LoadPurchasedPlayerModel()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string purchasedPlayerModel = PlayerPrefs.GetString(PlayerPrefsKey);
            string[] purchasedPlayerModelIndices = purchasedPlayerModel.Split(',');
            for (int i = 0; i < purchasedPlayerModelIndices.Length; i++)
            {
                int playerModelIndex;
                if (int.TryParse(purchasedPlayerModelIndices[i], out playerModelIndex))
                {
                    if (playerModelIndex >= 0 && playerModelIndex < _playerDataModels.Length)
                    {
                        _playerDataModels[playerModelIndex].isPurchased = true;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid player model index: " + playerModelIndex);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to parse player model index: " + purchasedPlayerModelIndices[i]);
                }
            }
        }
    }

    public void SaveIsSelectedPlayerModel()
    {
        PlayerPrefs.SetInt("CurrentModelIndex", _currentModelIndex);

        for (int i = 0; i < _playerDataModels.Length; i++)
        {
            string key = $"PlayerModel{i}_IsSelected";
            int isSelectedValue = _playerDataModels[i].isSelected ? 1 : 0;
            PlayerPrefs.SetInt(key, isSelectedValue);
        }

        PlayerPrefs.Save();
    }
    public void LoadIsSelectedPlayerModel()
    {
        _currentModelIndex = PlayerPrefs.GetInt("CurrentModelIndex");

        for (int i = 0; i < _playerDataModels.Length; i++)
        {
            string key = $"PlayerModel{i}_IsSelected";
            int isSelectedValue = PlayerPrefs.GetInt(key);
            _playerDataModels[i].isSelected = isSelectedValue == 1;
        }
    }
    private void LoadSelectedPlayerModelIndex()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            _currentModelIndex = data.SelectedPlayerModelIndex;
        }
    }
    #endregion
}