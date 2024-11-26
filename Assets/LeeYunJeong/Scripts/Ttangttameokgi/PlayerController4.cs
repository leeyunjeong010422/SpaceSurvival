using Photon.Pun;
using UnityEngine;

public class PlayerController4 : MonoBehaviourPun
{
    [SerializeField] float speed;

    private Rigidbody rb;

    [SerializeField] Animator animator;

    [Header("카메라 관련")]
    private Camera mainCamera;
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 5, -7); //카메라 기본 위치 (임의 설정)
    [SerializeField] float mouseSensitivity = 2f; //감도

    private float cameraPitch; //카메라 위아래 회전 각도 설정할 때 사용
    private Vector3 playerPosition; //플레이어 위치
    private Quaternion playerRotation; //플레이어 회전

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        mainCamera = Camera.main;

        if (photonView.IsMine)
        {
            UpdateCameraPosition();
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        Move();
        RotateView();
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir != Vector3.zero)
        {
            Vector3 worldMoveDir = transform.TransformDirection(moveDir).normalized;
            rb.velocity = new Vector3(worldMoveDir.x * speed, rb.velocity.y, worldMoveDir.z * speed);
            animator.SetFloat("Speed", 3);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetFloat("Speed", 0);
        }

        photonView.RPC("SyncAnimation", RpcTarget.Others, animator.GetFloat("Speed"));
    }

    private void RotateView()
    {
        if (!photonView.IsMine)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 회전 처리
        transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -10f, 20f); //카메라 위아래 각도 제한

        UpdateCameraPosition();

        // 회전 동기화를 위한 RPC 호출
        photonView.RPC("SyncRotation", RpcTarget.Others, transform.rotation);
    }

    private void UpdateCameraPosition()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position + Quaternion.Euler(cameraPitch, transform.eulerAngles.y, 0) * cameraOffset;
            mainCamera.transform.LookAt(transform.position + Vector3.up * 1.5f); //플레이어 약간 위를 바라봄
        }
    }

    [PunRPC]
    private void SyncAnimation(float speed) // 애니메이션 동기화 (움직임)
    {
        animator.SetFloat("Speed", speed);
    }

    [PunRPC]
    private void SyncRotation(Quaternion rotation) // 회전 동기화
    {
        transform.rotation = rotation;
    }
}
