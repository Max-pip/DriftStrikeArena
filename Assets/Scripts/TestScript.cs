using UnityEngine;

public class TestScript : MonoBehaviour
{
    public BonusSpawn bonusSpawn;

    private void Start()
    {
        bonusSpawn = FindAnyObjectByType<BonusSpawn>();
    }
}
