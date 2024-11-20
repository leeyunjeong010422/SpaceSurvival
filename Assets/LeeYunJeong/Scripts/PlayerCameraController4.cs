using UnityEngine;

public class PlayerCameraController4 : MonoBehaviour
{
    [Header("카메라 관련")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float rotationSpeed = 5f; // 카메라 회전 속도
    [SerializeField] private float mouseSensitivity = 2f; // 마우스 감도
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 5, -7); // 카메라 기본 위치 (플레이어 뒤쪽 약간 위쪽)
    private float cameraPitch = 0f; // 카메라 위아래 회전 각도

    private Transform player; // 플레이어의 Transform

    private void Start()
    {
        // 카메라가 플레이어의 자식으로 설정되어 있어야 함
        player = transform;

        // 카메라가 지정되지 않았다면, 현재 오브젝트의 카메라 사용
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
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
        if (playerCamera != null)
        {
            // 카메라 위치 업데이트: 플레이어 위치 + 카메라 오프셋 적용
            playerCamera.transform.position = player.position + Quaternion.Euler(cameraPitch, player.eulerAngles.y, 0) * cameraOffset;
            playerCamera.transform.LookAt(player.position + Vector3.up * 1.5f); // 플레이어 약간 위쪽을 바라보도록 설정
        }
    }
}
