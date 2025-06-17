using UnityEngine;

public class ObjectFloater : MonoBehaviour
{
    [SerializeField] float hoverHeight = 2f;
    [SerializeField] float hoverForce = 50f;
    [SerializeField] float maxDistanceModifier = 2f;
    [SerializeField] LayerMask groundLayer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MaintainHoverHeight();
    }

    void MaintainHoverHeight()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight * maxDistanceModifier, groundLayer))
        {
            float distanceToGround = hit.distance;
            float heightError = hoverHeight - distanceToGround;

            float upwardSpeed = rb.velocity.y;
            float lift = heightError * hoverForce - upwardSpeed * 2.0f;

            rb.AddForce(Vector3.up * lift, ForceMode.Acceleration);
        }
        else
        {
            // 地面が検出できないときはゆっくり落ちる
            rb.AddForce(Vector3.down * hoverForce, ForceMode.Acceleration);
        }
    }
}

