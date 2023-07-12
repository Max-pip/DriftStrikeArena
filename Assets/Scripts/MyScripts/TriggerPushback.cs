using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerPushback : MonoBehaviour
{
    public static UnityEvent<float> onTouchedTriggerByEnemy = new UnityEvent<float>();

    private float _damageModifier;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 rigidbodyForward = transform.forward;

        CarController enemyCarController = other.GetComponentInParent<CarController>();
        Rigidbody enemyRigidbody = other.GetComponentInParent<Rigidbody>();


        if (enemyCarController != null)
        {
            onTouchedTriggerByEnemy.Invoke(enemyCarController.TakingDamage(1000f));
            _damageModifier = enemyCarController.ReceiveDamage;
        }


        if (enemyRigidbody != null)
        {
            Vector3 forceDirection = (rigidbodyForward + new Vector3(0, 2f, 0)).normalized;
            enemyRigidbody.AddForce(forceDirection * _damageModifier, ForceMode.Impulse); 
        }
    }
}
