using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint3 : MonoBehaviourPun
{
    public static CheckPoint3 Instance;
    public int TotalCheckPoints { get; private set; } = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController3 playerController = other.GetComponent<PlayerController3>();

            if (playerController != null && playerController.photonView.IsMine && !playerController.visitedCheckPoint.Contains(this))
            {
                playerController.OnTriggerCheckPoint(this);
            }
        }
    }
}
