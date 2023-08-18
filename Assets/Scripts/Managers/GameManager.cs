using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isPaused = false;
    public bool isGameOver = false;

    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private Transform _playerStartPosition;

    public GameObject explosionPrefab;
    public GameObject playerGameObject { get; private set; }

    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

    public List<GameObject> allCars = new List<GameObject>();

    public List<GameObject> allEnemyCars = new List<GameObject>();

    [SerializeField] private List<GameObject> _enemyPrefabs = new List<GameObject>();

    private BonusSpawn _bonusSpawn;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AllForStart();
    }

    private void AllForStart()
    {
        _playerPrefab = MainManager.Instance.PlayerPrefab;

        playerGameObject = Instantiate(_playerPrefab, _playerStartPosition.position, Quaternion.identity);

        allCars.Add(playerGameObject);

        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, _enemyPrefabs.Count);

            GameObject enemyCarPrefab = Instantiate(_enemyPrefabs[randomIndex], _spawnPoints[i].position, Quaternion.Euler(new Vector3(0, _spawnPoints[i].eulerAngles.y, 0)));
            enemyCarPrefab.tag = $"Enemy{i}";

            CarControllerAI carControllerAI = enemyCarPrefab.GetComponent<CarControllerAI>();
            if (carControllerAI != null)
            {
                carControllerAI.defaultTarget = playerGameObject.transform;
            }

            MeshCollider enemyMeshColliderComponent = enemyCarPrefab.GetComponentInChildren<MeshCollider>();

            if (enemyMeshColliderComponent != null)
            {
                enemyMeshColliderComponent.tag = $"Enemy{i}";
                carControllerAI.tagName = $"Enemy{i}";
            }

            allEnemyCars.Add(enemyCarPrefab);
            allCars.Add(enemyCarPrefab);
        }

        _bonusSpawn = FindAnyObjectByType<BonusSpawn>();
        if (_bonusSpawn != null)
        {
            _bonusSpawn.Initialization();
        }
    }
}
