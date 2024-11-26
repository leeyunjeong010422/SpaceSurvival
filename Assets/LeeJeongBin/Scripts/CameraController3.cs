using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController3 : MonoBehaviour
{
    [SerializeField] float sensitivityX = 2f;
    [SerializeField] float sensitivityY = 2f;
    [SerializeField] float minYAngle = -60f;
    [SerializeField] float maxYAngle = 60f;
    [SerializeField] Transform player;
    [SerializeField] Camera playerCamera;

    private float rotationX = 0f;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // 포톤뷰가 해당 클라이언트 플레이어 소유일 경우 카메라 활성화 및 커서 숨기기
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerCamera.gameObject.SetActive(true); // 내 카메라 활성화
        }
        else
        {
            playerCamera.gameObject.SetActive(false); // 다른 플레이어 카메라 비활성화
        }
    }

    void Update()
    {
        // 본인 카메라일 경우에만 회전
        if (photonView.IsMine)
        {
            HandleCameraRotation();
        }
    }

    // 카메라 회전 처리
    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // X 축 회전 처리
        rotationX -= mouseY * sensitivityY;
        rotationX = Mathf.Clamp(rotationX, minYAngle, maxYAngle);

        // 카메라 회전 적용
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Y축 회전 (플레이어 회전 처리)
        player.Rotate(Vector3.up * mouseX * sensitivityX);  // X축은 카메라 회전, Y축은 플레이어 회전
    }
}
