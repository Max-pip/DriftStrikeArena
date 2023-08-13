using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBonusTrigger : MonoBehaviour
{
    private const string CarLayerName = "Vehicle";
    private const string TriggerPushbackTag = "TriggerPushback";

    [SerializeField] private HomingRocket _prefabHomingRocket;

    private void OnTriggerEnter(Collider other)
    {
        MeshCollider carMesh = other.GetComponent<MeshCollider>();

        if (other.gameObject.layer == LayerMask.NameToLayer(CarLayerName) && carMesh != null)
        {
            int randomIndex = Random.Range(0, GameManager.Instance.allEnemyCars.Count);
            HomingRocket homingRocket = Instantiate(_prefabHomingRocket, transform.position, _prefabHomingRocket.transform.rotation);
            homingRocket.Initialization(GameManager.Instance.allEnemyCars[randomIndex]);
        }
        Destroy(gameObject, 0.2f);
    }
}
