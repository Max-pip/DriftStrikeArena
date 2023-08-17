using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptTrigger : MonoBehaviour
{
    private const string CarLayerName = "Vehicle";
    private const string TriggerPushbackTag = "TriggerPushback";

    [SerializeField] private Mine _prafabMine;
    [SerializeField] private HomingRocket _prefabHomingRocket;
    [SerializeField] private GameObject _target;

    /*
    private void OnTriggerEnter(Collider other)
    {
        MeshCollider carMesh = other.GetComponent<MeshCollider>();

        if (other.gameObject.layer == LayerMask.NameToLayer(CarLayerName) && carMesh != null)
        {
            //int randomIndex = Random.Range(0, GameManager.Instance.allEnemyCars.Count);
            HomingRocket homingRocket = Instantiate(_prefabHomingRocket, transform.position, _prefabHomingRocket.transform.rotation);
            homingRocket.Initialization(_target);
        }
        Destroy(gameObject, 0.2f);
    }
    */

    
    private void OnTriggerEnter(Collider other)
    {
        MeshCollider carMesh = other.GetComponent<MeshCollider>();

        if (other.gameObject.layer == LayerMask.NameToLayer(CarLayerName) && carMesh != null)
        {
            Mine mine = Instantiate(_prafabMine, transform.position, Quaternion.identity);
            mine.Initialization();
        }
        Destroy(gameObject, 0.1f);
    }
    
}
