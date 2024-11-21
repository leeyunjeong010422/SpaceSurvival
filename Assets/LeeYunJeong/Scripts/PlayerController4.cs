using Photon.Pun;
using UnityEngine;

public class PlayerController4 : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] int maxHealth = 100;

    private bool isGrounded = false;
    private Rigidbody rb;

    private int currentHealth;
    private int score = 0;

    private Camera mainCamera;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;
        score = 0;

        mainCamera = Camera.main;

        if (photonView.IsMine)
        {
            // PlayerController4를 로컬 플레이어의 TagObject로 설정
            PhotonNetwork.LocalPlayer.TagObject = this;
        }
        UpdateProfileInfo();
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        Move();

        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // 점프는 바닥에 있을 때만
        {
            Jump();
        }
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir != Vector3.zero)
        {
            Vector3 worldMoveDir = transform.TransformDirection(moveDir).normalized;
            rb.velocity = new Vector3(worldMoveDir.x * speed, rb.velocity.y, worldMoveDir.z * speed);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //충돌한 면이 바닥인 경우 (normal.y > 0.7f로 바닥 확인)
        //Collision타입은 충돌 지점들의 정보를 담는 ContactPoint 타입의 데이터를 contacs라는 배열의 형태로 제공
        //여러 충돌지점중에서 첫번째 충돌지점의 정보를 가져옴
        if (collision.contacts[0].normal.y > 0.7f)
        {
            isGrounded = true;
        }
    }

    [PunRPC]
    private void Fire()
    {
        RaycastHit hit;

        // 카메라의 중앙 조준점을 기준으로 레이캐스트 발사
        Vector3 aimDirection = mainCamera.transform.forward; // 카메라가 바라보는 방향
        if (Physics.Raycast(mainCamera.transform.position, aimDirection, out hit))
        {
            PlayerController4 hitPlayer = hit.collider.GetComponent<PlayerController4>();
            if (hitPlayer != null && hitPlayer.photonView.IsMine == false)
            {
                // 50 데미지를 줌
                hitPlayer.photonView.RPC("TakeDamage", RpcTarget.All, 50, photonView.ViewID); // 공격자는 현재 플레이어
            }
        }
    }


    // 프로필 정보 업데이트
    private void UpdateProfileInfo()
    {
        PlayerProfileManager4 profileManager = FindObjectOfType<PlayerProfileManager4>();
        if (profileManager != null)
        {
            profileManager.UpdateProfileInfo(photonView.OwnerActorNr - 1, score, currentHealth);
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, int attackerViewID)
    {
        // 맞고 있는 자신만 처리
        if (!photonView.IsMine)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            PhotonView attackerPhotonView = PhotonView.Find(attackerViewID);
            if (attackerPhotonView != null)
            {
                string killerName = attackerPhotonView.Owner.NickName; // 공격자의 닉네임
                string victimName = photonView.Owner.NickName; // 피해자의 닉네임

                // 킬 로그를 네트워크에 동기화
                KillLogManager killLogManager = FindObjectOfType<KillLogManager>();
                if (killLogManager != null)
                {
                    killLogManager.AddKillLogNetwork(killerName, victimName);
                }

                IncreaseScore(attackerViewID); // 공격자에게 점수 추가
            }

            Die();
        }

        UpdateProfileInfo();
    }

    // 공격자가 플레이어를 죽였을 때 점수를 추가하는 함수
    private void IncreaseScore(int attackerViewID)
    {
        // 공격자 PhotonView를 찾고, 공격자에게 점수를 추가
        PhotonView attackerPhotonView = PhotonView.Find(attackerViewID);
        if (attackerPhotonView != null)
        {
            PlayerController4 attackerController = attackerPhotonView.GetComponent<PlayerController4>();
            if (attackerController != null)
            {
                attackerController.photonView.RPC("AddScore", RpcTarget.All, 50);
            }
        }
    }

    private void Die()
    {
        if (photonView.IsMine)
        {
            transform.position = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));  // 리스폰

            currentHealth = maxHealth;

            UpdateProfileInfo();
        }
    }

    [PunRPC]
    public void AddScore(int points) // 점수 추가하는 함수
    {
        score += points; // 점수 증가
        UpdateProfileInfo();

        // 점수 동기화
        photonView.RPC("SyncScore", RpcTarget.All, score);
    }

    [PunRPC]
    private void SyncScore(int newScore) // 점수 동기화하는 함수
    {
        score = newScore; // 점수 갱신
        UpdateProfileInfo();
    }

    public int GetScore() => score;
    public int GetHealth() => currentHealth;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(currentHealth);
            stream.SendNext(score);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            currentHealth = (int)stream.ReceiveNext();
            score = (int)stream.ReceiveNext();
        }
    }
}
