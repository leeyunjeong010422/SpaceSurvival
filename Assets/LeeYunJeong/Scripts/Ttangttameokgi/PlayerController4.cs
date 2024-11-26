using Photon.Pun;
using UnityEngine;

public class PlayerController4 : MonoBehaviourPun
{
    [SerializeField] float speed;

    private Rigidbody rb;

    [SerializeField] Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        Move();
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

    [PunRPC]
    private void SyncAnimation(float speed) // 애니메이션 동기화 (움직임)
    {
        animator.SetFloat("Speed", speed);
    }
}
