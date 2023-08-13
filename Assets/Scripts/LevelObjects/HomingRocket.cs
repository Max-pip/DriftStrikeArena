using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HomingRocket : MonoBehaviour
{
    private const string CarLayerName = "Vehicle";

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private GameObject _targetObject;
    private Collider _myCollider;

    public void Initialization(GameObject targetObject) 
    {
        _targetObject = targetObject;
        
    }

    private void Start()
    {
        _myCollider = GetComponentInChildren<Collider>();
        _myCollider.enabled = false;
        StartCoroutine(PersecutionOfTargetCoroutine());        
    }

    private IEnumerator PersecutionOfTargetCoroutine()
    {
        float duration = 1f;
        float normalizedTime = 0;

        while (normalizedTime <= duration )
        {
            _rb.velocity = transform.forward * _speed;
            normalizedTime += Time.deltaTime;

            yield return null;
        }

        _myCollider.enabled = true;
        _speed *= 2;
        _rotationSpeed *= 2;

        while (true)
        {
            _rb.velocity = transform.forward * _speed;

            RotateRocket();
        
            yield return null;
        }
        
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
            Destroy(gameObject);
        }
    }
}
