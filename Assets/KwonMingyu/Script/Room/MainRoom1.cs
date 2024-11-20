using Photon.Pun;
using UnityEngine;

public class MainRoom1 : MonoBehaviour
{
    [SerializeField] GameObject player;
    void Start()
    {
        PhotonNetwork.Instantiate(player.name, Vector3.zero, Quaternion.identity);
    }
}
