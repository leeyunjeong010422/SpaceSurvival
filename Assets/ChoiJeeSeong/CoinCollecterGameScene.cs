using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollecterGameScene : MonoBehaviourPunCallbacks
{
    [SerializeField] MiniGameScore scoreManager;

    private void Start()
    {
        // 이미 방에 들어가 있고
        // 게임에 참여할 플레이어가 모두 방에 들어가있어야 한다
        if (false == PhotonNetwork.InRoom)
        {
            Debug.LogWarning("방에 참여해 있지 않습니다");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            ReadyNetworkScene();
        }

        ReadyPlayerClient();
    }

    private void ReadyPlayerClient()
    {
        PhotonNetwork.Instantiate("Character2", new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)), Quaternion.identity);
        
        scoreManager.ReadyScoreTable();
    }

    private void ReadyNetworkScene()
    {

    }
}
