using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    [SerializeField] private float _modifierForRotate = 0.1f;

    [SerializeField] private CarController _carController;
    private Vector3 _initRotation;

    // Start is called before the first frame update
    void Start()
    {
        _initRotation = transform.localEulerAngles;  // Rotation relative to parent (car)
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate this according to the rotation input value
        float rotate = _carController.TurnValue * _carController.initialRotate * _modifierForRotate;
        transform.localEulerAngles = _initRotation + new Vector3(0f, rotate, 0f);
    }
}
