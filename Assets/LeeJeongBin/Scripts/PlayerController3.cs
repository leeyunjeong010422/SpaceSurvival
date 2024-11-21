using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController3 : MonoBehaviourPun
{
    [Header("이동 설정")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
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
        else
        {
            velocity = Vector3.zero;
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
            velocity = Vector3.zero;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}
