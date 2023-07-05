using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

    public List<GameObject> allCars = new List<GameObject>();

    public Action<Transform> OnDefaultTargetPositionSet;

    [SerializeField] private List<GameObject> _enemyPrefabs = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, _enemyPrefabs.Count);

            GameObject enemyCarPrefab = Instantiate(_enemyPrefabs[randomIndex], _spawnPoints[i].position, Quaternion.identity);

            CarControllerAI carControllerAI = enemyCarPrefab.GetComponent<CarControllerAI>();
            if (carControllerAI != null)
            {
                carControllerAI.defaultTarget = allCars[UnityEngine.Random.Range(0, allCars.Count)].transform;
            }

            MeshCollider enemyMeshColliderComponent = enemyCarPrefab.GetComponentInChildren<MeshCollider>();

            if (enemyMeshColliderComponent != null)
            {
                enemyMeshColliderComponent.tag = $"Enemy{i}";
                carControllerAI.tagName = $"Enemy{i}";
            }

            allCars.Add(enemyCarPrefab);
        }
    }
}
