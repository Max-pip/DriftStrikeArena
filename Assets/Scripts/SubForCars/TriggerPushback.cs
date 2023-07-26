
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerPushback : MonoBehaviour
{
    public static UnityEvent<float> onTouchedTriggerByEnemy = new UnityEvent<float>();

    [SerializeField] private AudioSource _damageAudio;

    private float _damageModifier;

    //For player
    private GameObject _cameraObject;
    private FollowCamera _followCamera;

    private Transform _transformParent;
    private CarController _carControllerPlayer;
    private float _currentSpeedPlayer;
    private float _minValueSlowMo = 18f;

    private float _slowMoDelay = 0.3f;
    private bool _isSlowMotionRunning = false;

    private void Start()
    {
        _transformParent = transform.parent;
        if (_transformParent != null && _transformParent.CompareTag("Player"))
        {
            _carControllerPlayer = _transformParent.GetComponent<CarController>();
            _cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            _followCamera = _cameraObject.GetComponent<FollowCamera>();
        }
    }

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
            if (_transformParent.CompareTag("Player") && !_isSlowMotionRunning)
            {
                _followCamera.ShakeCamera();
                _currentSpeedPlayer = _carControllerPlayer.rigidbody.velocity.magnitude;
                if (_currentSpeedPlayer > _minValueSlowMo)
                {
                    StartCoroutine(SlowMotionCoroutine());
                }
            }
            GameObject explosion = Instantiate(GameManager.Instance.explosionPrefab, transform.position, transform.rotation);
            Vector3 forceDirection = (rigidbodyForward + new Vector3(0, 2f, 0)).normalized;
            enemyRigidbody.AddForce(forceDirection * _damageModifier, ForceMode.Impulse); 

            if (MainManager.Instance.isSoundOn)
            {
                _damageAudio.Play();
            }

            Destroy(explosion, 1f);
        }
    }

    private IEnumerator SlowMotionCoroutine()
    {
        _isSlowMotionRunning = true;

        _followCamera.zoom = new Vector3(10, -10, 0);

        yield return new WaitForSeconds(0.05f);

        Time.timeScale = 0.2f;

        yield return new WaitForSeconds(_slowMoDelay);

        Time.timeScale = 1f;

        _followCamera.zoom = new Vector3(0, 0, 0);

        _isSlowMotionRunning = false;
    }
}
