using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;

    [Tooltip("Use scene's current setup for offset.")]
    public bool UseInitial = false;

    [Tooltip("Local offset vs target.")]
    public Vector3 Relative = new Vector3(0, 0, 0);    // Above & Forward, look-up the front-side

    [Tooltip("Global offset vs target.")]
    public Vector3 Offset = new Vector3(0, 5, -5);

    [Tooltip("Static rotation target (degree).")]
    public Vector3 Rotation = new Vector3(30, 0, 0);    // Example for looking down (degree)

    [Tooltip("Approximately the time it will take to reach the target.")]
    public float smoothTime = 0.3F;

    private Vector3 relative;
    private Vector3 offset;
    private Vector3 rotation;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        target = GameManager.Instance.playerGameObject.transform;

        if (UseInitial)
        {
            relative = Relative;
            offset = transform.position - target.transform.position;   // Vector operation
            rotation = transform.rotation.eulerAngles;
        }
        else
        {
            relative = Relative;
            offset = Offset;
            rotation = Rotation;
        }
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver == true)
        {
            return;
        }
        // Define a target position relative to the the target transform
        Vector3 targetPosition = target.TransformPoint(relative) + offset;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        //transform.rotation = Quaternion.Euler(rotation);
    }


}
