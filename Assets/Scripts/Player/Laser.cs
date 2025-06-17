using System;
using System.Collections;
using UnityEngine;

public class Laser : AttackerToEnemy
{
    [SerializeField] float speed = 30f;
    [SerializeField] GameObject laserHitVFX;
    [Tooltip("Laser will be destroyed after this time")]
    [SerializeField] float lifeTime = 5f;

    Rigidbody rb;
    float elapedTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        elapedTime += Time.deltaTime;
        if (elapedTime < lifeTime) return;
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CollisionComponent collisionComponent = other.transform.GetComponentInParent<CollisionComponent>();

        if (!collisionComponent) return;

        if (collisionComponent.Contains(CollisionType.DestroyPlayerLaser)) OnLaserHit();
    }

    void OnLaserHit()
    {
        Instantiate(laserHitVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
