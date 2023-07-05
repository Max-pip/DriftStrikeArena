using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControllerAI : MonoBehaviour
{
    [SerializeField] private Transform _targetPositionTransform;
    [SerializeField] private float _searchRadius = 90f;
    [SerializeField] private LayerMask _vehicleLayerMask;

    private GameManager _gameManager;
    private CarController _carController;

    private float _forwardAmound = 0f;
    private float _turnAmound = 0f;

    private Vector3 _targetPosition;

    private Collider[] _enemies;

    public Transform testTargetDefaultTransform;

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

    private void Awake()
    {
        //_gameManager = GetComponent<GameManager>();
        _gameManager = FindObjectOfType<GameManager>();
        _carController = GetComponent<CarController>();
    }

    private void Start()
    {
        _enemies = new Collider[4];
        //_targetPosition = _targetPositionTransform.position;
        _gameManager.OnDefaultTargetPositionSet += SetTargetPosition;
    }

    private void SetTargetPosition(Transform targetPosition)
    {
        testTargetDefaultTransform = targetPosition;
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, _searchRadius);
    }
    */
    

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

            if (enemy.gameObject.CompareTag(_tagName) || enemy.gameObject.CompareTag("Untagged"))
            {
                continue;
            }

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            //Debug.Log($"Closest enemy: {closestEnemy.name}");
            _targetPosition = closestEnemy.position;
        } else
        {
            //_targetPosition = _targetPositionTransform.position;
            //_targetPosition = testTargetDefaultTransform.position;
            if (_defaultTarget != null)
            {
                _targetPosition = _defaultTarget.position;
            }
        }
    }

    private void CommandMoveToEnemy()
    {
        Vector3 dirToMovePosition = (_targetPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToMovePosition);

        float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);

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
}
