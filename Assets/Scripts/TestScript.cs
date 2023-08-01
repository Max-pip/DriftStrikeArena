using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    private float _rotationSpeed = 4f;

    public IEnumerator DefaultZTurnRotationCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        rigidbody.useGravity = false;
        //rigidbody = true;
        transform.position += Vector3.up * 3;


        while (Quaternion.Angle(transform.rotation, targetRotation) > 5f) // Використовуємо метод Quaternion.Angle
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            yield return null;
        }


        //transform.rotation = targetRotation;
        rigidbody.useGravity = true;
    }
}
