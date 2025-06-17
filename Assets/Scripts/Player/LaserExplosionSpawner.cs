using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserExplosionSpawner : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlaySE("BurstLaserExplosion");
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

}
