using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Mine : MonoBehaviour
{
    private const string CarLayerName = "Vehicle";

    [SerializeField] private VisualEffect _explosionEffect;
    [SerializeField] private LayerMask _carLayer;
    [SerializeField] private LayerMask _blockExplosionLayer;
    [SerializeField] private float _radiusExplosion;
    [SerializeField] private int _maxColliderAmount = 10;
    [SerializeField] private float _explosiveForce;
    [SerializeField] private float _pointForce = 6000f;
    private Collider[] _hitsArray;
    private Collider _myCollider;

    private Rigidbody _enemyRigidbody;

    public void Initialization()
    {
        _myCollider = GetComponent<Collider>();
        StartCoroutine(DelayActivationMineCoroutine());
        _hitsArray = new Collider[_maxColliderAmount];
        FindGroundSpawnPosition();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radiusExplosion);
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, new Vector3(0, -10f, 0), Color.magenta);
    }

    private void FindGroundSpawnPosition()
    {
        RaycastHit spawnHit;

        float distance = 10f;

        Vector3 targetLocation;

        if (Physics.Raycast(transform.position, Vector3.down, out spawnHit, distance, _blockExplosionLayer))
        {
            targetLocation = spawnHit.point + new Vector3(0, transform.localScale.y / 2, 0);

            transform.position = targetLocation;
        }
    }

    private void Explosion()
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, _radiusExplosion, _hitsArray, _carLayer);

        for (int i = 0; i < hits; i++)
        {
            if (_hitsArray[i].TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                float distance = Vector3.Distance(transform.position, _hitsArray[i].transform.position);

                if (!Physics.Raycast(transform.position, (_hitsArray[i].transform.position - transform.position).normalized, distance, _blockExplosionLayer))
                {
                    rigidbody.AddExplosionForce(_explosiveForce, transform.position, _radiusExplosion, 2f);
                }
            }
        }
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        _enemyRigidbody = other.GetComponentInParent<Rigidbody>();

        if (other.gameObject.layer == LayerMask.NameToLayer(CarLayerName) && _enemyRigidbody != null)
        {
            StartCoroutine(DelayExplosionCoroutine());
            Destroy(gameObject, 0.3f);
        }
    }

    private IEnumerator DelayExplosionCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        _enemyRigidbody.AddForceAtPosition(Vector3.up * _pointForce, transform.position, ForceMode.Impulse);
        Explosion();
    }

    private IEnumerator DelayActivationMineCoroutine()
    {
        _myCollider.enabled = false;

        yield return new WaitForSeconds(1f);

        _myCollider.enabled = true;
    }
}
