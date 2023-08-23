using System.Collections;
using UnityEngine;

public class LevelBonusTrigger : MonoBehaviour
{
    private const string CarLayerName = "Vehicle";
    private const string CutOffHeightReference = "_CutoffHeight";

    [SerializeField] private HomingRocket _prefabHomingRocket;
    [SerializeField] private Mine _prefabMine;
    [SerializeField] private Material _myMaterial;
    private Material _materialLocal;

    public void Initialization()
    {
        _materialLocal = new Material(_myMaterial);
        _materialLocal = GetComponent<Renderer>().material;
        StartCoroutine(AppearanceCoroutine());
        //_materialLocal.SetFloat(CutOffHeightReference, 3.5f);
    } 

    private void OnTriggerEnter(Collider other)
    {
        MeshCollider carMesh = other.GetComponent<MeshCollider>();

        if (other.gameObject.layer == LayerMask.NameToLayer(CarLayerName) && carMesh != null)
        {
            int randomPrefab = Random.Range(0, 2);
            int randomCarIndex = Random.Range(0, (GameManager.Instance.allCars.Count - 1));
            Debug.Log($"randomPrefab {randomPrefab}. random index {randomCarIndex} from all cars count {GameManager.Instance.allCars.Count}");


            switch (randomPrefab)
            {
                case 0:
                    HomingRocket homingRocket = Instantiate(_prefabHomingRocket, transform.position, _prefabHomingRocket.transform.rotation);
                    homingRocket.Initialization(GameManager.Instance.allCars[randomCarIndex]);
                    StartCoroutine(DissableCoroutine());
                    break;
                case 1:
                    Mine mine = Instantiate(_prefabMine, transform.position, _prefabMine.transform.rotation);
                    mine.Initialization();
                    StartCoroutine(DissableCoroutine());
                    break;
                default:
                    Mine mineDefault = Instantiate(_prefabMine, transform.position, _prefabMine.transform.rotation);
                    mineDefault.Initialization();
                    StartCoroutine(DissableCoroutine());
                    break;
            }
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

    private IEnumerator AppearanceCoroutine()
    {
        _materialLocal.SetFloat(CutOffHeightReference, 0f);
        float valueCutoffHeight = _materialLocal.GetFloat(CutOffHeightReference);
        float duration = 0.25f;
        float time = 0.05f;

        while (time < duration)
        {
            float currentValue = Mathf.Lerp(valueCutoffHeight, 3.5f, time / duration);
            _materialLocal.SetFloat(CutOffHeightReference, currentValue);

            time += Time.deltaTime;
            yield return null;
        }
    }
}
