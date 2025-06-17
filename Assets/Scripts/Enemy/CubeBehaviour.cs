using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    Vector3 gravity = new(0, -40f, 0);
    Material material;
    Rigidbody body;

    float colorProgress = 0;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        body = GetComponent<Rigidbody>();
        body.velocity = new(0, 20, 0);
    }

    void Update()
    {
        if (colorProgress < 1)
        {
            material.color = Color.Lerp(Color.red, Color.white, colorProgress);
            colorProgress += Time.deltaTime;
            body.velocity += gravity * Time.deltaTime;
        }
    }
}
