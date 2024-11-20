using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController3 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float rotationSpeed;

    private CharacterController characterController;
    private Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        if (characterController.isGrounded)
        {
            velocity.y = 0f;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
}
