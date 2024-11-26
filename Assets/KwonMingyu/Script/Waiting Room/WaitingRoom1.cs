using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaitingRoom1 : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerCard1[] playerCards;
    [SerializeField] TMP_Text countdownText;

    private Coroutine GameStartCounterCoroutine;

    // 게임 승리 점수 설정 버튼 호스트만
    [SerializeField] GameObject roomSettingButtons;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            localPlayer.SetReady(!localPlayer.GetReady());
        }
    }

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

        roomSettingButtons.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        // 모든 플레이어가 Ready && 플레이어가 2명 이상일 때 게임 시작 카운트다운
        if (CheckAllReady() && PhotonNetwork.CurrentRoom.Players.Count > 1)
        {
            if (GameStartCounterCoroutine != null) return;
            GameStartCounterCoroutine = StartCoroutine(StartCountDownCoroutine());
        }
    }

    // 플레이어가 나가면 카드 업데이트
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCards();
        PlayerColorSet();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomSettingButtons.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
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
    }
    private void PlayerColorSet()
    {
        // 모든 포톤뷰를 순회 하면서 소유자의 색갈로 변경
        foreach (PhotonView photonView in FindObjectsOfType<PhotonView>())
        {
            Renderer renderer = photonView.gameObject.GetComponentInChildren<Renderer>();
            if (renderer == null) continue;
            renderer.material.color = photonView.Owner.GetNumberColor();
        }
        // 카드의 아웃라인 색갈을 플레이어 색갈로 변경
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            playerCards[player.GetPlayerNumber()].CardOutLineSet(player.GetNumberColor());
        }

    }
    IEnumerator StartCountDownCoroutine()
    {
        YieldInstruction waitCountDown = new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            // 플레이어가 레디를 풀거나 인원이 2명보다 적으면 카운트다운 중지
            if (!CheckAllReady() || PhotonNetwork.CurrentRoom.Players.Count < 2)
            {
                countdownText.gameObject.SetActive(false);
                StopCoroutine(GameStartCounterCoroutine);
                GameStartCounterCoroutine = null;
            }
            countdownText.text = (5 - i).ToString();
            yield return waitCountDown;
        }
        countdownText.gameObject.SetActive(false);
        GameStart();
    }

    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
            if (!player.GetReady()) return false;
        return true;
    }
    public void GameStart()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;
        MinigameSelecter.Instance.ResetRandomList();
        PhotonNetwork.LoadLevel(MinigameSelecter.Instance.PopRandomSceneIndex());
    }
}
