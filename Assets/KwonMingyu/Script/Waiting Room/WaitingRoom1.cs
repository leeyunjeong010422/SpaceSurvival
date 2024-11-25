using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
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
        // 플레이어 카드 업데이트
        UpdatePlayerCards();
        // 플레이어의 컬러가 기본값이라면 PlayerNuber를 부여
        if (targetPlayer.GetColorNumber() == -1)
        {
            targetPlayer.SetColorNumber(targetPlayer.GetPlayerNumber());
            return;
        }
        // 색갈 업데이트
        PlayerColorSet();
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
    private void PlayerColorSet()
    {
        // 모든 포톤뷰를 순회 하면서 소유자의 색갈로 변경
        foreach (PhotonView photonView in FindObjectsOfType<PhotonView>())
        {
            photonView.gameObject.GetComponent<Renderer>().material.color = photonView.Owner.GetNuberColor();
        }

    }

    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
            if (!player.GetReady()) return false;
        return true;
    }
    public void GameStart()
    {

    }
}
