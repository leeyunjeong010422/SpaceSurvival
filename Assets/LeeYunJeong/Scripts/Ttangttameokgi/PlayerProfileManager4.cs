using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileManager4 : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] profileCards;
    [SerializeField] TMP_Text[] nickNameTexts;
    [SerializeField] TMP_Text[] scoreTexts; // 본인 땅 개수 (점수)
    [SerializeField] Color myProfileColor = default; // 내 프로필 카드 색상

    private Dictionary<int, int> playerScores = new Dictionary<int, int>(); // ActorNumber별 점수 저장

    private void Awake()
    {
        // Hex 색상을 Color로 변환
        if (ColorUtility.TryParseHtmlString("#FF9090", out var color))
        {
            myProfileColor = color;
        }
    }

    private void Start()
    {
        // 모든 프로필 카드를 비활성화
        foreach (var card in profileCards)
        {
            card.SetActive(false);
        }
    }

    public override void OnJoinedRoom()
    {
        // 프로필 카드 초기화
        InitializeProfileCards();

        // 로컬 플레이어의 정보 가져 와서 내 프로필 정보를 업데이트
        PlayerController4 playerController = PhotonNetwork.LocalPlayer.TagObject as PlayerController4;
        if (playerController != null)
        {
            //UpdateProfileInfo(PhotonNetwork.LocalPlayer.ActorNumber - 1, );
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 플레이어가 입장할 때마다 프로필 카드 활성화
        InitializeProfileCards();
    }

    // 플레이어가 나가면 카드 업데이트
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 플레이어가 나가면 해당 플레이어의 데이터를 삭제하고 UI 업데이트
        if (playerScores.ContainsKey(otherPlayer.ActorNumber))
        {
            playerScores.Remove(otherPlayer.ActorNumber);
        }
        InitializeProfileCards();
    }

    private void InitializeProfileCards()
    {
        int playerCount = PhotonNetwork.PlayerList.Length; // 현재 방에 있는 플레이어 수

        for (int i = 0; i < 4; i++)
        {
            profileCards[i].SetActive(i < playerCount);

            if (i < playerCount)
            {
                // 각 플레이어의 닉네임 가져오기
                Player player = PhotonNetwork.PlayerList[i];
                nickNameTexts[i].text = player.NickName;

                // 내 프로필 카드 색상 변경
                if (PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber)
                {
                    profileCards[i].GetComponent<Image>().color = myProfileColor;
                }
                // 초기 점수 설정 (없으면 0점)
                if (!playerScores.ContainsKey(player.ActorNumber))
                {
                    playerScores[player.ActorNumber] = 0;
                }

                // 점수 표시
                scoreTexts[i].text = $"점수: {playerScores[player.ActorNumber]}";
            }
        }
    }

    // 프로필 정보 업데이트
    public void UpdateProfileInfo(int playerIndex, int score)
    {
        // 점수 저장
        int actorNumber = PhotonNetwork.PlayerList[playerIndex].ActorNumber;
        playerScores[actorNumber] = score;

        // UI 업데이트
        scoreTexts[playerIndex].text = $"점수: {score}";
    }
}
