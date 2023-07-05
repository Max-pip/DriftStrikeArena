using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
        {
            Vector3 rbForward = transform.forward;
            //rbForward.y = 0f;

            Rigidbody otherRigidbody = other.GetComponentInParent<Rigidbody>();

            otherRigidbody.AddForce(rbForward * 5000f, ForceMode.Impulse);
        }
    }
}
