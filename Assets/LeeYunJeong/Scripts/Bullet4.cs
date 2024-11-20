using Photon.Pun;
using UnityEngine;

public class Bullet4 : MonoBehaviourPun
{
    [SerializeField] float speed = 20f;
    [SerializeField] float lifetime = 2f;

    private Rigidbody rb;
    private PhotonView attackerView; // 공격자의 PhotonView

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime); // 총알 파괴
    }

    // 공격자 설정
    public void SetAttacker(Photon.Realtime.Player player)
    {
        attackerView = player.TagObject is PlayerController4 playerController ? playerController.photonView : null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController4 playerController = collision.gameObject.GetComponent<PlayerController4>();
            if (playerController != null && attackerView != null)
            {
                Debug.Log("TakeDamage 호출됨");
                PhotonView targetPhotonView = playerController.photonView;
                targetPhotonView.RPC("TakeDamage", RpcTarget.All, 10, attackerView.ViewID); // 공격자의 ViewID 사용
            }
            Destroy(gameObject); // 충돌 후 총알 파괴
        }
    }
}
