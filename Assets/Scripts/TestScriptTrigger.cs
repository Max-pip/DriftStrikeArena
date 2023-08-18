using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptTrigger : MonoBehaviour
{
    private const string CarLayerName = "Vehicle";
    private const string CutOffHeightReference = "_CutoffHeight";

    [SerializeField] private Mine _prafabMine;
    [SerializeField] private HomingRocket _prefabHomingRocket;
    [SerializeField] private GameObject _target;
    [SerializeField] private Material _myMaterial;
    private Material _materialLocal;

    private void Start()
    {
        _materialLocal = new Material(_myMaterial);
        _materialLocal = GetComponent<Renderer>().material;
        _materialLocal.SetFloat(CutOffHeightReference, 3.5f);
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        MeshCollider carMesh = other.GetComponent<MeshCollider>();

        if (other.gameObject.layer == LayerMask.NameToLayer(CarLayerName) && carMesh != null)
        {
            HomingRocket homingRocket = Instantiate(_prefabHomingRocket, transform.position, _prefabHomingRocket.transform.rotation);
            homingRocket.Initialization(_target);
            StartCoroutine(DissableCoroutine());
        }
    }
    */
    
    

    
    private void OnTriggerEnter(Collider other)
    {
        MeshCollider carMesh = other.GetComponent<MeshCollider>();

        if (other.gameObject.layer == LayerMask.NameToLayer(CarLayerName) && carMesh != null)
        {
            Mine mine = Instantiate(_prafabMine, transform.position, Quaternion.identity);
            mine.Initialization();
            StartCoroutine(DissableCoroutine());
        }
    }
    

    private IEnumerator DissableCoroutine()
    {
        float valueCutoffHeight = _materialLocal.GetFloat(CutOffHeightReference);
        float duration = 0.4f;
        float time = 0.05f;

        while (time < duration)
        {
            float currentValue = Mathf.Lerp(valueCutoffHeight, 0, time / duration);
            _materialLocal.SetFloat(CutOffHeightReference, currentValue);

            time += Time.deltaTime;
            yield return null;
        }

        _materialLocal.SetFloat(CutOffHeightReference, 0);
        Destroy(gameObject);
    }
    
}
