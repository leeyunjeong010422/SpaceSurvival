using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoom1 : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerCard1[] playerCards;
    [SerializeField] Button startButton;

    // 플레이어의 정보가 업데이트 될 때 (플레이어의 Room number가 지정될 때)
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 룸 넘버가 할당되지 않았다면 리턴
        if (targetPlayer.GetPlayerNumber() == -1) return;

        // 방 최대 인원수 만큼 카드를 활성화 후 리셋
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            playerCards[i].gameObject.SetActive(true);
            playerCards[i].CardInfoReset();
        }

        // 플레이어 룸 넘버의 카드에 정보를 입력
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            playerCards[player.GetPlayerNumber()].CardInfoCanger(player);
        }

        startButton.interactable = CheckAllReady() && PhotonNetwork.LocalPlayer.IsMasterClient;
    }

    // 나간 플레이어의 카드에 정보를 리셋
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerCards[otherPlayer.GetPlayerNumber()].CardInfoReset();
    }

    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
            if (!player.GetReady()) return false;
        return true;
    }
    public void GameStart(int scene)
    {
        PhotonNetwork.LoadLevel(scene);
    }
}
