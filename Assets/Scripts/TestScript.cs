using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class TestScript : MonoBehaviour
{
    [SerializeField] private VisualEffect _testEffect;

    private void Start()
    {
        StartCoroutine(TestCoroutine());
    }

    private IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            VisualEffect testEffect = Instantiate(_testEffect, transform.position, Quaternion.identity);
            Destroy(testEffect.gameObject, 1f);

            yield return new WaitForSeconds(3f);
        }
    }
}
