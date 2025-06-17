using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehaviour : MonoBehaviour
{
    [SerializeField] float LimitRadius;
    [SerializeField] GameObject hitVFX;

    Rigidbody body;
    Vector3 spawnedPosition;

    public void Init(Transform player, float speed)
    {
        body = GetComponent<Rigidbody>();
        body.velocity = speed * (player.position - transform.position).normalized;

        spawnedPosition = transform.position;
    }

    void FixedUpdate()
    {
        if ((spawnedPosition - transform.position).magnitude > LimitRadius) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        CollisionComponent collisionComponent = other.transform.GetComponentInParent<CollisionComponent>();

        if (!collisionComponent) return;

        if (collisionComponent.Contains(CollisionType.DestroyEnemyBullet)) OnHit();
    }

    void OnHit()
    {
        Instantiate(hitVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
