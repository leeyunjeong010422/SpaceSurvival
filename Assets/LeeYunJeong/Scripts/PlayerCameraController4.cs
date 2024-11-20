using Photon.Pun;
using UnityEngine;

public class PlayerCameraController4 : MonoBehaviourPun
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -7); // 카메라 기본 위치 (플레이어 뒤쪽 약간 위쪽)
    private float cameraPitch = 0f; // 카메라 위아래 회전 각도

    private Transform player;

    private void Start()
    {
        player = transform;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (!photonView.IsMine)
        {
            playerCamera.gameObject.SetActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            RotateView();
            UpdateCameraPosition();
        }
    }

    private void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 수평 회전
        player.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime);

        // 수직 회전 (카메라 위아래 회전 제한)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -20f, 10f); // 카메라 위아래 각도 제한
    }

    private void UpdateCameraPosition()
    {
        if (playerCamera != null)
        {
            playerCamera.transform.position = player.position + Quaternion.Euler(cameraPitch, player.eulerAngles.y, 0) * cameraOffset;
            playerCamera.transform.LookAt(player.position + Vector3.up * 1.5f); // 플레이어 약간 위쪽을 바라보도록 설정
        }
    }
}
