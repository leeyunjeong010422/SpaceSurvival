using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class CameraController3 : MonoBehaviour
{
    [SerializeField] float sensitivityX = 2f;
    [SerializeField] float sensitivityY = 2f;
    [SerializeField] float minYAngle = -60f;
    [SerializeField] float maxYAngle = 60f;
    [SerializeField] Transform player;
    [SerializeField] Camera playerCamera;
    [SerializeField] float distancePlayer;
    [SerializeField] float cameraHeight;

    private float rotationX = 0f;
    private PhotonView photonView;
    private CameraController3 killerCamera;
    private bool isDead = false;

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

        Vector3 targetPosition = player.position - transform.forward * distancePlayer + Vector3.up * cameraHeight;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);

        // 카메라 회전 적용
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Y축 회전 (플레이어 회전 처리)
        player.Rotate(Vector3.up * mouseX * sensitivityX);  // X축은 카메라 회전, Y축은 플레이어 회전
    }

    public void DeadPlayer(PhotonView killerPhotonView)
    {
        if (photonView.IsMine)
        {
            isDead = true;

            if (killerPhotonView != null)
            {
                GameObject cameraObject = killerPhotonView.gameObject.transform.Find("Camera").gameObject;
                CameraController3 killerCamera = cameraObject.GetComponent<CameraController3>();
                if (killerCamera != null)
                {
                    SetCameraKiller(killerCamera);
                }
            }
        }
    }

    private void SetCameraKiller(CameraController3 killerCamera)
    {
        playerCamera.gameObject.SetActive(false);

        killerCamera.playerCamera.gameObject.SetActive(true);
    }
}
