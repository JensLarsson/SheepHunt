using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 10;
    [SerializeField] float distanceFromTarget = 2;
    [SerializeField] float minMaxPitch = 80f;

    [SerializeField] float rotationSmoothTime = 0.12f;

    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float yaw;
    float pitch;

    

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentRotation = Vector3.SmoothDamp(
            currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        currentRotation.x = Mathf.Clamp(currentRotation.x, -minMaxPitch, minMaxPitch);
        transform.eulerAngles = currentRotation;
    }
}
