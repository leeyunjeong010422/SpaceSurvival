using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollecterGameScene : MiniGameSceneBase
{
    [SerializeField] MiniGameScore scoreManager;

    protected override void ReadyNetworkScene()
    {

    }

    protected override void ReadyPlayerClient()
    {
        PhotonNetwork.Instantiate("Character2", new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)), Quaternion.identity);
        
        scoreManager.InitScoreTable();
    }

}
