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

    public int playerScore = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        mainCamera = Camera.main;

        // 내 PlayerController를 LocalPlayer에 Tag로 연결
        if (photonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.TagObject = this; // LocalPlayer에 현재 PlayerController 연결

            UpdateCameraPosition();

            SetPlayerColor();

            // 나중에 입장한 플레이어에게도 RPC 호출
            photonView.RPC(nameof(SyncPlayerColor), RpcTarget.AllBuffered, playerColor.r, playerColor.g, playerColor.b);
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

        photonView.RPC(nameof(SyncAnimation), RpcTarget.Others, animator.GetFloat("Speed"));
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

        photonView.RPC(nameof(SyncRotation), RpcTarget.Others, transform.rotation);
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
        //최종적으로 사용할 코드
        //playerColor = PhotonNetwork.LocalPlayer.GetNumberColor();
        //photonView.RPC(nameof(SyncPlayerColor), RpcTarget.AllBuffered, playerColor.r, playerColor.g, playerColor.b);

        if (Application.isEditor)
        {
            playerColor = (PhotonNetwork.LocalPlayer.ActorNumber == 1) ? Color.red :
                          (PhotonNetwork.LocalPlayer.ActorNumber == 2) ? Color.blue :
                          (PhotonNetwork.LocalPlayer.ActorNumber == 3) ? Color.green : Color.yellow;
        }
        else
        {
            playerColor = PhotonNetwork.LocalPlayer.GetNumberColor();
        }

        photonView.RPC(nameof(SyncPlayerColor), RpcTarget.AllBuffered, playerColor.r, playerColor.g, playerColor.b);
    }

    [PunRPC]
    private void SyncPlayerColor(float r, float g, float b)
    {
        playerColor = new Color(r, g, b);
        if (playerRenderer != null)
        {
            playerRenderer.material.color = playerColor;  // 플레이어의 렌더러 색 변경
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            Renderer cubeRenderer = collision.gameObject.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                // 이미 색이 바뀐 큐브에서는 점수가 증가하지 않도록 처리
                if (cubeRenderer != null && cubeRenderer.material.color != playerColor)
                {
                    // 큐브 색 변경
                    cubeRenderer.material.color = playerColor;

                    // 색상 동기화만 처리 (점수는 동기화하지 않음)
                    photonView.RPC(nameof(SyncCubeColor), RpcTarget.All, collision.gameObject.GetInstanceID(), playerColor.r, playerColor.g, playerColor.b);

                    if (!photonView.IsMine) return; // 로컬 플레이어만 점수 처리

                    // 로컬 점수 증가
                    playerScore++;

                    // 점수를 다른 클라이언트에 동기화
                    photonView.RPC(nameof(SyncPlayerScore), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, playerScore);

                    // 로컬 UI 업데이트
                    UpdateLocalUI();
                }
            }
        }
    }

    // 큐브 색 동기화
    private void UpdateLocalUI()
    {
        PlayerProfileManager4 profileManager = FindObjectOfType<PlayerProfileManager4>();
        if (profileManager != null)
        {
            // 모든 클라이언트가 해당 플레이어의 UI를 업데이트
            profileManager.UpdateProfileInfo(PhotonNetwork.LocalPlayer.ActorNumber - 1, playerScore);
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
                cubeRenderer.material.color = new Color(r, g, b); // 모든 클라이언트에서 색상만 동기화
            }
        }
    }

    [PunRPC]
    private void SyncPlayerScore(int actorNumber, int score)
    {
        // 특정 플레이어(actorNumber)의 점수를 업데이트
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            playerScore = score; // 본인의 점수만 업데이트
        }

        // 모든 클라이언트에서 UI 업데이트
        PlayerProfileManager4 profileManager = FindObjectOfType<PlayerProfileManager4>();
        if (profileManager != null)
        {
            profileManager.UpdateProfileInfo(actorNumber - 1, score);
        }
    }

    private GameObject FindCubeByID(int cubeID)
    {
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
}
