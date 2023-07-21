using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerPushback : MonoBehaviour
{
    public static UnityEvent<float> onTouchedTriggerByEnemy = new UnityEvent<float>();

    [SerializeField] private AudioSource _damageAudio;

    private float _damageModifier;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 rigidbodyForward = transform.forward;

        CarController enemyCarController = other.GetComponentInParent<CarController>();
        Rigidbody enemyRigidbody = other.GetComponentInParent<Rigidbody>();


        if (enemyCarController != null)
        {
            onTouchedTriggerByEnemy.Invoke(enemyCarController.TakingDamage(800f));
            _damageModifier = enemyCarController.ReceiveDamage;
        }


        if (enemyRigidbody != null)
        {
            GameObject explosion = Instantiate(GameManager.Instance.explosionPrefab, transform.position, transform.rotation);
            Vector3 forceDirection = (rigidbodyForward + new Vector3(0, 2f, 0)).normalized;
            enemyRigidbody.AddForce(forceDirection * _damageModifier, ForceMode.Impulse); 
            _damageAudio.Play();
            Destroy(explosion, 1f);
        }
    }
}
