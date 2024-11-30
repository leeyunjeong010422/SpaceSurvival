using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeathCamera3 : MonoBehaviour
{
    private CameraController2 cameraController;
    public Camera playerCamera;
    private bool isDead = false;
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;

    private void Start()
    {
        // 기존 카메라 컨트롤러 참조
        cameraController = playerCamera.GetComponent<CameraController2>();

        if (cameraController != null)
        {
            // 기존 카메라 제어 비활성화
            cameraController.enabled = false;
        }

        // 플레이어가 죽었을 때만 카메라 제어 활성화
        if (isDead)
        {
            EnableFreeCamera();
        }
    }

    private void Update()
    {
        if (isDead)
        {
            HandleFreeCameraMovement();
        }
    }

    // 플레이어가 죽었을 때 자유 시점(PlayerController3 PlayerDestroy 메서드 호출 시)
    public void OnPlayerDead()
    {
        isDead = true;
        EnableFreeCamera();
    }

    private void EnableFreeCamera()
    {
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }
    }

    private void HandleFreeCameraMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // 마우스 카메라 회전
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(0, mouseX * lookSpeed, 0);

        // 마우스 Y축 이동
        Camera.main.transform.Rotate(-mouseY * lookSpeed, 0, 0);
    }
}
