using System;
using UnityEngine;

public class DeadZoneTrigger : MonoBehaviour
{
    public static Action onLosePanel;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject.name}");
        if (other.CompareTag("Player") && GameManager.Instance.isGameOver == false)
        {
            onLosePanel?.Invoke();
        }
    }
}
