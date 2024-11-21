using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController3 : MonoBehaviourPun
{
    [Header("플레이어 이동 설정")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float deceleration;
    [SerializeField] CharacterController characterController;
    [SerializeField] Vector3 velocity;

    private PhotonTransformView photonTransformView;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        photonTransformView = GetComponent<PhotonTransformView>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
        }

        if (photonTransformView != null)
        {
            photonTransformView.SetSynchronizedValues(velocity.x, velocity.y, velocity.z);
        }
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

            velocity.x = moveDirection.x * moveSpeed;
            velocity.z = moveDirection.z * moveSpeed;
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, 0, deceleration * Time.deltaTime);
        }

        if (characterController.isGrounded)
        {
            velocity.y = -0.5f;
        }
        else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}
