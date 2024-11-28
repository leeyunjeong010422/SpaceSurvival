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

    private GameSceneTest4 gameSceneTest;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        mainCamera = Camera.main;

        gameSceneTest = FindObjectOfType<GameSceneTest4>();

        // 내 PlayerController를 LocalPlayer에 Tag로 연결
        if (photonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.TagObject = this; // LocalPlayer에 현재 PlayerController 연결
            Debug.Log($"TagObject 설정완료: {PhotonNetwork.LocalPlayer.NickName}");
            photonView.RPC(nameof(SyncTagObject), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber); // 다른 플레이어에게 동기화

            UpdateCameraPosition();

            SetPlayerColor();

            // 나중에 입장한 플레이어에게도 RPC 호출
            photonView.RPC(nameof(SyncPlayerColor), RpcTarget.AllBuffered, playerColor.r, playerColor.g, playerColor.b);
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    [PunRPC]
    private void SyncTagObject(int actorNumber)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actorNumber)
            {
                player.TagObject = this;
                Debug.Log($"플레이어 {player.NickName}의 TagObject가 설정되었습니다.");
                break;
            }
        }
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
        playerColor = PhotonNetwork.LocalPlayer.GetNumberColor();
        photonView.RPC(nameof(SyncPlayerColor), RpcTarget.AllBuffered, playerColor.r, playerColor.g, playerColor.b);

        //if (Application.isEditor)
        //{
        //    playerColor = (PhotonNetwork.LocalPlayer.ActorNumber == 1) ? Color.red :
        //                  (PhotonNetwork.LocalPlayer.ActorNumber == 2) ? Color.blue :
        //                  (PhotonNetwork.LocalPlayer.ActorNumber == 3) ? Color.green : Color.yellow;
        //}
        //else
        //{
        //    playerColor = PhotonNetwork.LocalPlayer.GetNumberColor();
        //}

        //photonView.RPC(nameof(SyncPlayerColor), RpcTarget.AllBuffered, playerColor.r, playerColor.g, playerColor.b);
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
                // 다른 플레이어가 칠한 큐브를 밟으면 점수를 차감
                if (cubeRenderer.material.color != playerColor)
                {
                    // 큐브 색이 다른 플레이어의 색이라면 점수 차감
                    DecreaseScore(cubeRenderer.material.color);

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
                    UpdateLocalUI(PhotonNetwork.LocalPlayer.ActorNumber, playerScore);
                }
            }
        }
    }

    private void UpdateLocalUI(int actorNumber, int score)
    {
        PlayerProfileManager4 profileManager = FindObjectOfType<PlayerProfileManager4>();
        if (profileManager != null)
        {
            profileManager.UpdateProfileInfo(actorNumber - 1, score); // UI 동기화
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
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actorNumber)
            {
                PlayerController4 controller = player.TagObject as PlayerController4;
                if (controller != null)
                {
                    controller.playerScore = score;

                    // UI 업데이트
                    UpdateLocalUI(player.ActorNumber, score);
                }
            }
        }
    }

    private void DecreaseScore(Color cubeColor)
    {
        // 큐브를 칠한 플레이어의 색이 r, g, b와 일치하면 해당 플레이어의 점수를 차감
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.TagObject != null)
            {
                PlayerController4 controller = player.TagObject as PlayerController4;
                if (controller != null && controller.playerColor == cubeColor)
                {
                    Debug.LogWarning("점수가 차감되었습니다.");
                    controller.playerScore--; // 해당 플레이어의 점수 차감

                    // 점수 차감 후, 해당 플레이어의 점수를 모든 클라이언트와 동기화
                    photonView.RPC(nameof(SyncPlayerScore), RpcTarget.All, player.ActorNumber, controller.playerScore);
                }
            }
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
