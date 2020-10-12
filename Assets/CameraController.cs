using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] bool lockCursor;
    [SerializeField] float mouseSensitivity = 10;
    [SerializeField] MovementController target;
    [SerializeField] float distanceFromTarget = 2;
    [SerializeField] float scrollStrenght = 2;
    [SerializeField] Vector2 pitchMinMax = new Vector2(-40, 85);

    [SerializeField] float rotationSmoothTime = 0.12f;

    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float yaw;
    float pitch;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        distanceFromTarget -= scrollWheel * scrollStrenght;
        distanceFromTarget = Mathf.Clamp(distanceFromTarget, 1, 10);
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
        Vector3 cameraPos = target.transform.position - transform.forward * distanceFromTarget;
        cameraPos.y = Mathf.Max(cameraPos.y, 0);
        transform.position = cameraPos;

    }
}
