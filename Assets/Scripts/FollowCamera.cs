using System.Collections;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;

    [Tooltip("Approximately the time it will take to reach the target.")]
    public float smoothTime = 0.3f;

    public Vector3 zoom = new Vector3(0, 0, 0);

    public Vector3 startFollowPosition = new Vector3(-17f, 16, 0);

    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    //Camera shake
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.5f;

    private Vector3 _originalPos;
    private Quaternion _originalRotation;

    void Start()
    {
        target = GameManager.Instance.playerGameObject.transform;

        transform.position = target.position + startFollowPosition;

        offset = transform.position - target.transform.position;   // Vector operation
    }

    void Update()
    {
        
        if (GameManager.Instance.isGameOver == true)
        {
            return;
        }
        
        
        // Define a target position relative to the the target transform
        Vector3 targetPosition = target.position + offset + zoom;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void ShakeCamera()
    {
        _originalPos = transform.position;
        _originalRotation = transform.rotation;

        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0;

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = _originalPos + new Vector3(x, y, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = _originalPos;
        transform.rotation = _originalRotation;
    }
}
