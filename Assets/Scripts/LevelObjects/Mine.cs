using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Mine : ExplosionClass
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _pointForce = 6000f;
    private Collider _myCollider;

    private Rigidbody _enemyRigidbody;

    public void Initialization()
    {
        _myCollider = GetComponent<Collider>();
        StartCoroutine(DelayActivationMineCoroutine());
        FindGroundSpawnPosition();
    }

    private void FindGroundSpawnPosition()
    {
        RaycastHit spawnHit;

        float distance = 10f;

        Vector3 targetLocation;

        if (Physics.Raycast(transform.position, Vector3.down, out spawnHit, distance, _groundLayer))
        {
            targetLocation = spawnHit.point + new Vector3(0, transform.localScale.y / 2, 0);

            transform.position = targetLocation;
        }
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
