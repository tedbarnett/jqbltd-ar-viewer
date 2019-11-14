using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 1.0f;

    void Update()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smothedPosition;

        transform.LookAt(target);
    }
}