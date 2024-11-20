using Photon.Pun;
using UnityEngine;

public class PlayerController4 : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float speed;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] GameObject bulletPrefab;

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
        Vector3 moveDir = new Vector3();
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");

        if (moveDir == Vector3.zero)
            return;

        Vector3 worldMoveDir = transform.TransformDirection(moveDir).normalized;
        transform.position += worldMoveDir * speed * Time.deltaTime;
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
