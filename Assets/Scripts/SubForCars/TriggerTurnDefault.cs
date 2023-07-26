using System.Collections;
using UnityEngine;

public class TriggerTurnDefault : MonoBehaviour
{
    [SerializeField] private CarController _carController;

    private bool _isTurnOnGround = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _isTurnOnGround = true;
            StartCoroutine(CheckCurrentTouchGround());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _isTurnOnGround= false;
        }
    }


    private IEnumerator CheckCurrentTouchGround()
    {
        yield return new WaitForSeconds(1f);

        if (_isTurnOnGround)
        {
            _carController.StartDefaultTurnVahicleCoroutine();
        } 
    }
}
