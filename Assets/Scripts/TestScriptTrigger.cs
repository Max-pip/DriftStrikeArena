using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptTrigger : MonoBehaviour
{
    [SerializeField] private TestScript _testScript;

    private bool _isTurnOnGround = false;

    private void OnTriggerEnter(Collider other)
    {
            //_isTurnOnGround = true;
            StartCoroutine(_testScript.DefaultZTurnRotationCoroutine());
    }
}
