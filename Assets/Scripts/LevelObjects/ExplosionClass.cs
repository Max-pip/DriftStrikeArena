using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ExplosionClass : MonoBehaviour
{
    protected const string CarLayerName = "Vehicle";

    [SerializeField] private VisualEffect _explosionEffect;
    [SerializeField] private LayerMask _carLayer;
    [SerializeField] private LayerMask _blockExplosionLayer;
    [SerializeField] private float _radiusExplosion = 10f;
}
