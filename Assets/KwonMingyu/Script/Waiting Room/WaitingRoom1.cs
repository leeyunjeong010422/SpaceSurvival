using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoom1 : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerCard1[] playerCards;
    [SerializeField] Button startButton;
    [SerializeField] TMP_Text selectGameName;
    private int miniGameSceneNumber;
    private string[] miniGameNames = { "랜덤", "동전줍기", "라스트맨스탱딩", "TPS게임" };

    // 플레이어의 정보가 업데이트 될 때 (플레이어의 Room number가 지정될 때)
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 룸 넘버가 할당되지 않았다면 리턴
        if (targetPlayer.GetPlayerNumber() == -1) return;
        // 플레이어 카드 업데이트
        UpdatePlayerCards();
    }

    // 플레이어가 나가면 카드 업데이트
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCards();
    }

    public void UpdatePlayerCards()
    {
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

        // 모든 플레이어가 Ready && 마스터 클라이언트 라면 시작버튼 활성화
        startButton.interactable = CheckAllReady() && PhotonNetwork.LocalPlayer.IsMasterClient;
    }

    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
            if (!player.GetReady()) return false;
        return true;
    }
    public void GameStart()
    {
        if (miniGameSceneNumber == 0)
        {
            miniGameSceneNumber = Random.Range(1, 4);
        }
        PhotonNetwork.LoadLevel(miniGameSceneNumber);
    }
    public void MiniGameChange(bool up)
    {
        miniGameSceneNumber += up ? 1 : -1;
        miniGameSceneNumber = Mathf.Clamp(miniGameSceneNumber, 0, 3);
        selectGameName.text = miniGameNames[miniGameSceneNumber];
    }
}
