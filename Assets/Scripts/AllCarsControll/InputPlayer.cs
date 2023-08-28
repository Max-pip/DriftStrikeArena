﻿using UnityEngine;

public class InputPlayer : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    private const string Vectical = "Vertical";
    private const string Horizontal = "Horizontal";
    //private const string Throttle = "Throttle";
    //private const string Sideways = "Sideways";

    private void FixedUpdate()
    {
        MobileInput();
    }

    private void MobileInput()
    {
        _carController.ForwardValue = SimpleInput.GetAxisRaw(Vectical);
        _carController.TurnValue = SimpleInput.GetAxisRaw(Horizontal);
    }
    /*
    private void KeybordInput()
    {
        _carController.ForwardValue = Input.GetAxisRaw(Throttle);
        _carController.TurnValue = Input.GetAxisRaw(Sideways);
    }
    */
}
