using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TPSPlayerController4 : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float speed;
    //[SerializeField] float jumpForce;
    [SerializeField] int maxHealth = 100;
    private bool isDead = false;
    private bool isMoving = false;

    //private bool isGrounded = false;
    private Rigidbody rb;

    private int currentHealth;
    private int score = 0;

    private PlayerCameraController4 cameraController;
    private Camera mainCamera;

    [SerializeField] Animator animator;

    [SerializeField] GameObject countdownCanvas;
    [SerializeField] TMP_Text countdownText;

    [SerializeField] GameObject bulletHole;

    private GameObject takeDamagePanel;

    [Header("사운드 관련")]
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip takeDamageSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;
        score = 0;

        cameraController = FindObjectOfType<PlayerCameraController4>();
        mainCamera = Camera.main;

        animator = GetComponent<Animator>();

        UpdateProfileInfo();

        if (photonView.IsMine)
        {
            // PlayerController4를 로컬 플레이어의 TagObject로 설정
            PhotonNetwork.LocalPlayer.TagObject = this;

            if (countdownCanvas == null)
            {
                countdownCanvas = GameObject.Find("CountdownCanvas");
            }

            if (countdownText == null && countdownCanvas != null)
            {
                countdownText = countdownCanvas.GetComponentInChildren<TMP_Text>();
            }

            if (countdownCanvas != null)
            {
                countdownCanvas.SetActive(false);
            };

            takeDamagePanel = GameObject.Find("Canvas/TakeDamagePanel");
            takeDamagePanel?.SetActive(false);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        Move();

        if (Input.GetButtonDown("Fire1") && !Input.GetKey(KeyCode.LeftControl))
        {
            Fire();
        }

        if (Input.GetKey(KeyCode.LeftShift) && isMoving)
        {
            Run();
        }

        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // 점프는 바닥에 있을 때만
        //{
        //    Jump();
        //}
    }

    private void Move()
    {
        if (isDead) return;

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir != Vector3.zero)
        {
            isMoving = true;
            Vector3 worldMoveDir = transform.TransformDirection(moveDir).normalized;
            rb.velocity = new Vector3(worldMoveDir.x * speed, rb.velocity.y, worldMoveDir.z * speed);
            animator.SetFloat("Speed", 3);
        }
        else
        {
            isMoving = false;
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetFloat("Speed", 0);
        }

        photonView.RPC("SyncAnimation", RpcTarget.Others, animator.GetFloat("Speed"));
    }

    private void Run()
    {
        if (isDead) return;

        animator.SetFloat("Speed", 6);
        photonView.RPC("SyncAnimation", RpcTarget.Others, animator.GetFloat("Speed"));
    }

    //private void Jump()
    //{
    //    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //    isGrounded = false;

    //    photonView.RPC("SyncTrigger", RpcTarget.Others, "Jump");
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //충돌한 면이 바닥인 경우 (normal.y > 0.7f로 바닥 확인)
    //    //Collision타입은 충돌 지점들의 정보를 담는 ContactPoint 타입의 데이터를 contacs라는 배열의 형태로 제공
    //    //여러 충돌지점중에서 첫번째 충돌지점의 정보를 가져옴
    //    if (collision.contacts[0].normal.y > 0.7f)
    //    {
    //        isGrounded = true;
    //    }
    //}

    [PunRPC]
    private void Fire()
    {
        if (isDead) return; // 죽었을 때는 공격 불가

        RaycastHit hit;

        // 애니메이션 레이어 1 활성화
        animator.SetLayerWeight(1, 1f);
        photonView.RPC("SetLayerWeight", RpcTarget.Others, 1, 1f);

        animator.SetTrigger("Fire4");
        photonView.RPC("SyncTrigger", RpcTarget.All, "Fire4");

        // 카메라의 중앙 조준점을 기준으로 레이캐스트 발사
        Vector3 aimDirection = mainCamera.transform.forward; // 카메라가 바라보는 방향
        if (Physics.Raycast(mainCamera.transform.position, aimDirection, out hit))
        {
            TPSPlayerController4 hitPlayer = hit.collider.GetComponent<TPSPlayerController4>();
            if (hitPlayer != null && hitPlayer.photonView.IsMine == false)
            {
                // 50 데미지를 줌
                hitPlayer.photonView.RPC("TakeDamage", RpcTarget.All, 50, photonView.ViewID); // 공격자는 현재 플레이어
            }

            if (!hit.collider.CompareTag("Player"))
            {
                CreateBulletHole(hit);
            }
        }

        // 총알 발사음 동기화
        photonView.RPC("PlayFireSound", RpcTarget.All);

        // 1초 후에 레이어를 비활성화하는 메서드 호출
        Invoke("DisableFireLayer", 1f);
    }

    private void CreateBulletHole(RaycastHit hit)
    {
        // 충돌한 위치와 방향을 계산하여 총알 자국을 배치
        Vector3 bulletHolePosition = hit.point + hit.normal * 0.01f; // 표면에 약간 떠있게 배치
        Quaternion bulletHoleRotation = Quaternion.LookRotation(hit.normal); // 표면과 일치하도록 회전

        GameObject createdBulletHole = PhotonNetwork.Instantiate(bulletHole.name, bulletHolePosition, bulletHoleRotation);

        Destroy(createdBulletHole, 0.5f);
    }

    // 1초 후에 애니메이션 레이어를 비활성화하는 메서드
    private void DisableFireLayer()
    {
        animator.SetLayerWeight(1, 0f);
        photonView.RPC("SetLayerWeight", RpcTarget.Others, 1, 0f);
    }


    // 프로필 정보 업데이트
    private void UpdateProfileInfo()
    {
        TPSPlayerProfileManager4 profileManager = FindObjectOfType<TPSPlayerProfileManager4>();
        if (profileManager != null)
        {
            profileManager.UpdateProfileInfo(photonView.Owner.GetPlayerNumber(), score, currentHealth);
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, int attackerViewID)
    {
        // 맞고 있는 자신만 처리
        if (!photonView.IsMine || IsGameEnded())
            return;

        GameManager.Sound.PlaySFX(takeDamageSound, 5);

        if (isDead) return;

        takeDamagePanel?.SetActive(true);

        currentHealth -= damage;

        if (cameraController != null)
        {
            StartCoroutine(cameraController.CameraShake(0.2f, 0.1f)); // 지속시간 0.2초, 흔들림 강도 0.1
        }

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
        Invoke("TakeDamagePanelDelete", 0.3f);
    }

    private void TakeDamagePanelDelete()
    {
        takeDamagePanel?.SetActive(false);
    }

    // 공격자가 플레이어를 죽였을 때 점수를 추가하는 함수
    private void IncreaseScore(int attackerViewID)
    {
        // 공격자 PhotonView를 찾고, 공격자에게 점수를 추가
        PhotonView attackerPhotonView = PhotonView.Find(attackerViewID);
        if (attackerPhotonView != null)
        {
            TPSPlayerController4 attackerController = attackerPhotonView.GetComponent<TPSPlayerController4>();
            if (attackerController != null)
            {
                attackerController.photonView.RPC("AddScore", RpcTarget.All, 50);
            }
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die4");
        photonView.RPC("SyncTrigger", RpcTarget.Others, "Die4");

        if (photonView.IsMine)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        if (IsGameEnded()) yield break;

        countdownCanvas.SetActive(true);

        //죽은 플레이어에게 리스폰까지 몇초 남았는지 화면에 표시
        for (int i = 3; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = $"리스폰까지 {i}초 남았습니다...";
            }
            yield return new WaitForSeconds(1f);
        }

        countdownCanvas.SetActive(false);

        // NavMesh 위에서 랜덤 위치를 찾아 이동
        Vector3 respawnPosition = new Vector3(Random.Range(-30, 30), 0.5f, Random.Range(-30, 30));
        transform.position = respawnPosition;

        currentHealth = maxHealth;
        UpdateProfileInfo();

        //리스폰 되었을 때 애니메이션 초기화 (다시 기본 애니메이션으로 바꿈)
        isDead = false;
        animator.SetFloat("Speed", 0);
        animator.ResetTrigger("Die4");
        animator.SetTrigger("Idle4");
        photonView.RPC("SyncTrigger", RpcTarget.Others, "Idle4");
    }

    private Vector3 RandomPositionNavMesh(Vector3 center, float range)
    {
        // 임의의 위치 생성
        Vector3 randomPosition = center + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

        // NavMesh에서 유효한 위치인지 확인
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }

    [PunRPC]
    public void AddScore(int points) // 점수 추가 동기화
    {
        if (IsGameEnded()) return;

        score += points; // 점수 증가
        UpdateProfileInfo();

        // 점수 동기화
        photonView.RPC("SyncScore", RpcTarget.All, score);
    }

    [PunRPC]
    private void SyncScore(int newScore) // 점수 동기화
    {
        score = newScore; // 점수 갱신
        UpdateProfileInfo();
    }

    [PunRPC]
    private void SyncAnimation(float speed) // 애니메이션 동기화 (움직임)
    {
        animator.SetFloat("Speed", speed);
    }

    [PunRPC]
    private void SyncTrigger(string triggerName) // 애니메이션 트리거 동기화
    {
        animator.SetTrigger(triggerName);
    }

    [PunRPC]
    private void SetLayerWeight(int layerIndex, float weight) // 애니메이션 레이어 동기화
    {
        animator.SetLayerWeight(layerIndex, weight);
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

    private bool IsGameEnded()
    {
        TPSGameSceneTest4 gameScene = FindObjectOfType<TPSGameSceneTest4>();
        return gameScene != null && gameScene.isGameEnded;
    }

    [PunRPC]
    private void PlayFireSound()
    {
        GameManager.Sound.PlaySFX(fireSound, 0.8f);
    }
}
