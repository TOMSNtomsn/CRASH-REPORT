using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : AttackerToEnemy
{
    [SerializeField] float collisionLifeTime;
    [SerializeField] float effectLifeTime = 0.1f;

    float elapedTime = 0f;
    SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x / 2);
    }

    void Update()
    {
        elapedTime += Time.deltaTime;

        if (elapedTime >= collisionLifeTime && sphereCollider) sphereCollider.radius = 0;
        if (elapedTime >= effectLifeTime) Destroy(this.gameObject);
    }
}
