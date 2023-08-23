using System.Collections;
using UnityEngine;

public class HomingRocket : ExplosionClass
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private GameObject _targetObject;
    //[SerializeField] private float _pointForce = 26000;
    private Collider _myCollider;
    //private Rigidbody _enemyRigidbody;

    public void Initialization(GameObject targetObject) 
    {
        _targetObject = targetObject;
        _myCollider = GetComponentInChildren<Collider>();
        _myCollider.enabled = false;
        StartCoroutine(PersecutionOfTargetCoroutine());
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

        ExplosionEffect();
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
        _enemyRigidbody = collision.gameObject.GetComponentInParent<Rigidbody>();

        if (collision.gameObject.layer == LayerMask.NameToLayer(CarLayerName) && _enemyRigidbody != null)
        {
            ExplosionForce();
            ExplosionEffect();
            //_enemyRigidbody = null;
            Destroy(gameObject, 0.1f);
        }
    }  
}
