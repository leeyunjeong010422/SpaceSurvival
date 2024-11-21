using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcySlidingGameScene : MiniGameSceneBase
{
    private PlayerCharacterControl2 localPlayerCharacter;

    protected override void ReadyNetworkScene()
    {
    }

    protected override void ReadyPlayerClient()
    {
        GameObject instance = PhotonNetwork.Instantiate("Character2 IcySliding", new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)), Quaternion.identity);
        localPlayerCharacter = instance.GetComponent<PlayerCharacterControl2>();

        localPlayerCharacter.enabled = false; // 게임 시작 전까지 플레이어 컨트롤 비활성화

    }

    protected override void GameStart()
    {
        localPlayerCharacter.enabled = true;
    }
}
