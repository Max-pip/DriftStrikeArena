using System;
using UnityEngine;

public class AccelerationPlatform : MonoBehaviour
{
    public static Action onAddedAcceleration;

    private const string PlayerTag = "Player";
    private const string TriggerPushbackTag = "TriggerPushback";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            float angle = Vector3.Angle(transform.forward, other.transform.forward);
            if (angle <= 90)
            {
                onAddedAcceleration?.Invoke();
            }
        }
        else if (other.CompareTag("TriggerPushback"))
        {
            return;
        }
    }
}
