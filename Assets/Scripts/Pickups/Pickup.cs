using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 50f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    protected virtual void OnPickup(Transform palyer) { }

    protected virtual void OnTriggerEnter(Collider other)
    {
        CollisionComponent collisionComponent = other.GetComponentInParent<CollisionComponent>();

        if (!collisionComponent) return;

        if (collisionComponent.Contains(CollisionType.IsPlayer))
        {
            OnPickup(other.transform);
            Destroy(gameObject);
        }
    }
}
