using UnityEngine;

public class HammerImpact : MonoBehaviour
{
    [SerializeField] private float _damage = 500;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10f);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 forwardDirection = transform.forward;
        Vector3 forceVector = forwardDirection.normalized * _damage;

        if (collision.rigidbody != null )
        {
            collision.rigidbody.AddForce(forceVector, ForceMode.Impulse);
        }
    }
    */
}
