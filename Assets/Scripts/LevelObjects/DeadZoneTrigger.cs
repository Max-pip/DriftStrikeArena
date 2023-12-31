using System;
using UnityEngine;

public class DeadZoneTrigger : MonoBehaviour
{
    public static Action onLosePanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.isGameOver == false)
        {
            onLosePanel?.Invoke();
        }
    }
}
