using System;
using UnityEngine;

public class CarControllerAI : MonoBehaviour
{
    public static Action onWinPanel;

    [SerializeField] private Transform _targetPositionTransform;
    [SerializeField] private float _searchRadius = 80f;
    [SerializeField] private LayerMask _vehicleLayerMask;

    private CarController _carController;

    private float _forwardAmound = 0f;
    private float _turnAmound = 0f;

    private Vector3 _targetPosition;

    private Collider[] _enemies;

    private Transform _defaultTarget;

    public Transform defaultTarget 
    { 
        get { return _defaultTarget; }
        set { _defaultTarget = value; }
    }

    private string _tagName;

    public string tagName
    {
        get { return _tagName; }
        set { _tagName = value; }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }


    private void Awake()
    {
        _carController = GetComponent<CarController>();
    }

    private void Start()
    {
        _enemies = new Collider[6];
    }

    private void FixedUpdate()
    {
        SearchClosestEnemy();

        CommandMoveToEnemy();
    }

    private void SearchClosestEnemy()
    {
        int numEnemies = Physics.OverlapSphereNonAlloc(transform.position, _searchRadius, _enemies, _vehicleLayerMask);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        for (int i = 0; i < numEnemies; i++)
        {
            Collider enemy = _enemies[i];

            if (!enemy.CompareTag(_tagName) && !enemy.CompareTag("TriggerPushback") && !enemy.CompareTag("Ground") && !enemy.CompareTag("Untagged") && !enemy.CompareTag("DeadZone") && enemy.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemy.transform;
                }
            } 
        }

        if (closestEnemy != null)
        {
            _targetPosition = closestEnemy.position;
        } else
        {
            if (_defaultTarget != null)
            {
                _targetPosition = _defaultTarget.position;
            }
        }
    }

    private void CommandMoveToEnemy()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);

        Vector3 dirToMovePosition = (_targetPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToMovePosition);

        if (dot > 0)
        {
            _forwardAmound = 0.7f;
        }
        else
        {
            float reverseDistance = 25f;
            if (distanceToTarget > reverseDistance)
            {
                _forwardAmound = 0.7f;
            }
            else
            {
                _forwardAmound = -0.7f;
            }

        }

        float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);

        if (angleToDir > 0)
        {
            _turnAmound = 0.8f;
        }
        else
        {
            _turnAmound = -0.8f;
        }

        _carController.ForwardValue = _forwardAmound;
        _carController.TurnValue = _turnAmound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            GameManager.Instance.allEnemyCars.Remove(this.gameObject);
            if (GameManager.Instance.allEnemyCars.Count < 1 && GameManager.Instance.isGameOver == false)
            {
                onWinPanel?.Invoke();
            }
        }
    }
}
