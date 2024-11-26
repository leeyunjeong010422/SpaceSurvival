using Photon.Pun;
using System.Linq;
using UnityEngine;

public class PlayerController4 : MonoBehaviourPun
{
    [SerializeField] float speed;
    private Rigidbody rb;
    [SerializeField] Animator animator;

    [Header("카메라 관련")]
    private Camera mainCamera;
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 5, -7);
    [SerializeField] float mouseSensitivity = 2f;

    private float cameraPitch;
    private Vector3 playerPosition;
    private Quaternion playerRotation;

    [SerializeField] Renderer playerRenderer; // 플레이어의 렌더러 (큐브 색 변경을 위해)

    private Color playerColor; // 플레이어 색

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        mainCamera = Camera.main;

        if (photonView.IsMine)
        {
            UpdateCameraPosition();
            SetPlayerColor();
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

        transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -10f, 20f); // 카메라 위아래 각도 제한

        UpdateCameraPosition();

        photonView.RPC("SyncRotation", RpcTarget.Others, transform.rotation);
    }

    private void UpdateCameraPosition()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position + Quaternion.Euler(cameraPitch, transform.eulerAngles.y, 0) * cameraOffset;
            mainCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);
        }
    }

    private void SetPlayerColor()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber; // 입장 순서대로 플레이어에게 색을 부여
        switch (playerIndex)
        {
            case 1:
                playerColor = Color.red;
                break;
            case 2:
                playerColor = Color.blue;
                break;
            case 3:
                playerColor = Color.yellow;
                break;
            case 4:
                playerColor = Color.green;
                break;
        }

        // 플레이어의 색 설정
        //playerRenderer.material.color = playerColor;

        // 색 동기화
        photonView.RPC("SyncPlayerColor", RpcTarget.All, playerColor.r, playerColor.g, playerColor.b);
    }

    // 큐브 색을 변경하는 함수
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            Renderer cubeRenderer = collision.gameObject.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                // 큐브 색 변경
                cubeRenderer.material.color = playerColor;

                // 색 동기화
                photonView.RPC("SyncCubeColor", RpcTarget.All, collision.gameObject.GetInstanceID(), playerColor.r, playerColor.g, playerColor.b);
            }
        }
    }

    // 큐브 색 동기화
    [PunRPC]
    private void SyncCubeColor(int cubeID, float r, float g, float b)
    {
        GameObject cube = FindCubeByID(cubeID); // 큐브 ID로 큐브를 찾음
        if (cube != null)
        {
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = new Color(r, g, b);
            }
        }
    }

    // 큐브를 ID로 찾기
    private GameObject FindCubeByID(int cubeID)
    {
        // 큐브 ID로 해당 큐브를 찾기 위해 큐브를 GameObject 배열로 관리
        return GameObject.FindGameObjectsWithTag("Cube")
                          .FirstOrDefault(cube => cube.GetInstanceID() == cubeID);
    }

    // 애니메이션 동기화 (움직임)
    [PunRPC]
    private void SyncAnimation(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    // 회전 동기화
    [PunRPC]
    private void SyncRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    // 색 동기화 (다른 플레이어들에게 색을 적용)
    [PunRPC]
    private void SyncPlayerColor(float r, float g, float b)
    {
        playerColor = new Color(r, g, b); // 받은 값으로 Color 객체 재생성
        playerRenderer.material.color = playerColor;
    }
}
