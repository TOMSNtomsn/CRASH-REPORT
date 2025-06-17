using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 0.1f;

    Vector2 lastMousePos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            var x = lastMousePos.x - Input.mousePosition.x;

            transform.Rotate(0, x * rotationSpeed, 0);
            lastMousePos = Input.mousePosition;
        }
    }
}
