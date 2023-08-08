using System.Collections;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private GameObject _speedEffectObj;

    private void OnEnable()
    {
        AccelerationPlatform.onAddedAcceleration += TestMethod;      
    }

    private void OnDisable()
    {
        AccelerationPlatform.onAddedAcceleration -= TestMethod;
    }

    private void TestMethod() 
    {
        StartCoroutine(TestCoroutine());
    }    

    private IEnumerator TestCoroutine()
    {
        _speedEffectObj.SetActive(true);
        yield return new WaitForSeconds(2);
        _speedEffectObj.SetActive(false);
    }
}
