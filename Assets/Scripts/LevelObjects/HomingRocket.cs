using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class HomingRocket : MonoBehaviour
{
    private const string CarLayerName = "Vehicle";

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private GameObject _targetObject;
    private Collider _myCollider;

    [Header("Explosion paramenters")]
    [SerializeField] private VisualEffect _explosionEffect;
    [SerializeField] private int _maxColliderAmount = 15;
    [SerializeField] private float _radiusExplosion = 10f;
    [SerializeField] private LayerMask _carLayer;
    [SerializeField] private LayerMask _blockExplosionLayer;
    [SerializeField] private float _explosiveForce;
    private Collider[] _hitsArray; 

    public void Initialization(GameObject targetObject) 
    {
        _targetObject = targetObject;
        _myCollider = GetComponentInChildren<Collider>();
        _myCollider.enabled = false;
        _hitsArray = new Collider[_maxColliderAmount];
        StartCoroutine(PersecutionOfTargetCoroutine());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosiveForce);
    }

    private IEnumerator PersecutionOfTargetCoroutine()
    {
        float duration = 1f;
        float time = 0;

        while (time <= duration )
        {
            _rb.velocity = transform.forward * _speed;
            time += Time.deltaTime;

            yield return null;
        }

        _myCollider.enabled = true;
        _speed *= 2.5f;
        _rotationSpeed *= 2.5f;

        float liveDuration = 4;
        time = 0f;
        while (time <= liveDuration)
        {
            time += Time.deltaTime;
            _rb.velocity = transform.forward * _speed;

            RotateRocket();
        
            yield return null;
        }

        Explosion();
        Destroy(gameObject, 0.1f);
    }

    private void RotateRocket()
    {
        Vector3 heading = _targetObject.transform.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(heading);
        Quaternion smoothRotation = Quaternion.RotateTowards(_rb.rotation, rotation, _rotationSpeed * Time.deltaTime);
        _rb.MoveRotation(smoothRotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(CarLayerName))
        {
            Explosion();
            Destroy(gameObject, 0.1f);
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
}
