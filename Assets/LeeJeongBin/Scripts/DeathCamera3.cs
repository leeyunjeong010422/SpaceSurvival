using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeathCamera3 : MonoBehaviour
{
    private CameraController2 cameraController;
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;

    private void Start()
    {
        this.enabled = false;
    }

    private void Update()
    {
        HandleFreeCameraMovement();
    }

    // 플레이어가 죽었을 때 자유 시점(PlayerController3 PlayerDestroy 메서드 호출 시)
    public void OnPlayerDead()
    {
        EnableFreeCamera();
    }

    private void EnableFreeCamera()
    {
        this.enabled = true;
        GetComponent<CameraController2>().Target = null; // 타겟을 해제해서 더이상 무언가를 추적하지 않게 변경
    }

    private void HandleFreeCameraMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);

        //// 마우스 카메라 회전
        //float mouseX = Input.GetAxis("Mouse X");
        //float mouseY = Input.GetAxis("Mouse Y");

        //transform.Rotate(0, mouseX * lookSpeed, 0);

        //// 마우스 Y축 이동
        //Camera.main.transform.Rotate(-mouseY * lookSpeed, 0, 0);
    }
}
