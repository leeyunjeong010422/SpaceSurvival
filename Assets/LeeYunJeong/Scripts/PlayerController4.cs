using Photon.Pun;
using UnityEngine;

public class PlayerController4 : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float speed;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] int maxHealth = 100;

    private int currentHealth;
    private int score = 0;

    private void Start()
    {
        currentHealth = maxHealth;
        score = 0;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir != Vector3.zero)
        {
            Vector3 worldMoveDir = transform.TransformDirection(moveDir).normalized;
            transform.position += worldMoveDir * speed * Time.deltaTime;
        }
    }

    [PunRPC]
    private void Fire()
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, muzzlePoint.position, muzzlePoint.rotation);
        Bullet4 bulletScript = bullet.GetComponent<Bullet4>();
        bulletScript.SetAttacker(photonView.Owner); // 공격자 설정
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
