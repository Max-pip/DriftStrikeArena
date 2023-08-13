using UnityEngine;

public class HammerScript : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private Rigidbody _rb;

    private void Start()
    {
        //_rb.angularVelocity = new Vector3(_rotationSpeed, 0, 0);
    }

    /*
    private void Update()
    {
        
        Quaternion rotationX = Quaternion.Euler(_rotationSpeed * Time.deltaTime, 0f, 0f);

        transform.rotation = rotationX * transform.rotation;
        
        
        //transform.Rotate(Vector3.right * _rotationSpeed * Time.deltaTime);
    }
    */

    private void FixedUpdate()
    {
        _rb.AddRelativeTorque(500000, 0f, 0f);
    }
}
