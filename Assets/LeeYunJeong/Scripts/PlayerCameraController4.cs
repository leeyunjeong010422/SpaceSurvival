using Photon.Pun;
using UnityEngine;

public class PlayerCameraController4 : MonoBehaviourPun
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -7); // 카메라 기본 위치 (플레이어 뒤쪽 약간 위쪽)
    private float cameraPitch = 0f; // 카메라 위아래 회전 각도

    private Transform player;
    private Camera mainCamera;

    private void Start()
    {
        player = transform;

        mainCamera = Camera.main;

        if (!photonView.IsMine)
        {
            return; // 다른 플레이어의 경우 카메라를 동기화하지 않음
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!photonView.IsMine || mainCamera == null)
        {
            return;
        }

        RotateView();
        UpdateCameraPosition();
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
        if (mainCamera != null)
        {
            // 카메라 위치를 플레이어의 뒤쪽에 두고 조준점은 왼쪽으로 설정
            Vector3 cameraPosition = player.position + Quaternion.Euler(cameraPitch, player.eulerAngles.y, 0) * cameraOffset;

            // 조준점은 플레이어의 오른쪽으로 이동 (조준점이 항상 플레이어의 오른쪽 위치하도록 함)
            Vector3 aimPoint = player.position + Vector3.up * 1.5f - player.right * -1.5f;

            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.LookAt(aimPoint);
        }
    }
}
