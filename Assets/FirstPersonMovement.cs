using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] FirstPersonCamera camera;
    CharacterController characterController;
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, camera.transform.eulerAngles.y, 0);

        float forward = Input.GetAxis("Vertical");
        float right = Input.GetAxis("Horizontal");
        characterController.Move(transform.forward * forward * Time.deltaTime * speed);
        characterController.Move(transform.right * right * Time.deltaTime * speed);
        GameMaster.Instance.SetPlayerPosition(transform.position);
    }
}
