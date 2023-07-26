using System.Collections;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    private Transform _playerTransform;

    private Vector3 _relative;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _offset = new Vector3(0, 2, 0);

    private float _smoothTime = 0.01f; 

    private void Start()
    {
        //_playerTransform = GameManager.Instance.playerGameObject.transform;
        StartCoroutine(SetPlayerTransformCoroutine());
    }


    private void Update()
    {
        if (_playerTransform != null)
        {
            // Define a target position relative to the the target transform
            Vector3 targetPosition = _playerTransform.TransformPoint(_relative) + _offset;

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
        }
    }

    private IEnumerator SetPlayerTransformCoroutine()
    {
        yield return new WaitForSeconds(0.1f);

        _playerTransform = GameManager.Instance.playerGameObject.transform;
    }
}
