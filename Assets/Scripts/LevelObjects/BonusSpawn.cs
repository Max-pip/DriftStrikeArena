using System.Collections;
using UnityEngine;

public class BonusSpawn : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LevelBonusTrigger _bonusTrigger;
    [SerializeField] private float _pauseBetweenSpawn = 10;
    private Renderer _planeRenderer;
    private Vector3 _initialSpawnPosition;
    private Vector3 _targetLocation;

    public void Initialization()
    {
        _planeRenderer = GetComponent<Renderer>();
        if (GameManager.Instance != null)
        {
            StartCoroutine(SpawningBonusesCoroutine());
        }
    }

    private void FindSpawnBonusPosition()
    {
        _initialSpawnPosition = new Vector3(
            Random.Range(_planeRenderer.bounds.min.x, _planeRenderer.bounds.max.x),
            transform.position.y,
            Random.Range(_planeRenderer.bounds.min.z, _planeRenderer.bounds.max.z)
        );

        RaycastHit spawnHit;

        float distance = 20f;

        if (Physics.Raycast(_initialSpawnPosition, Vector3.down, out spawnHit, distance, _groundLayer))
        {
            _targetLocation = spawnHit.point + new Vector3(0, _bonusTrigger.transform.localScale.y / 2, 0);
        }
    }

    private void SpawnBonus()
    {
        LevelBonusTrigger bonusTrigger = Instantiate(_bonusTrigger, _targetLocation, Quaternion.identity);
        bonusTrigger.Initialization();
    }

    private IEnumerator SpawningBonusesCoroutine()
    {
        while (!GameManager.Instance.isGameOver)
        {
            FindSpawnBonusPosition();
            SpawnBonus();

            yield return new WaitForSeconds(_pauseBetweenSpawn);
        }
    }
}
