using UnityEngine;
using UnityEngine.VFX;

public class ExplosionClass : MonoBehaviour
{
    protected const string CarLayerName = "Vehicle";

    [SerializeField] private VisualEffect _explosionEffect;
    [SerializeField] private LayerMask _carLayer;
    [SerializeField] private LayerMask _blockExplosionLayer;
    [SerializeField] private float _radiusExplosion = 10f;
    [SerializeField] private float _explosiveForce;
    private Collider[] _hitsArray = new Collider[15];

    public void Explosion()
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
}
