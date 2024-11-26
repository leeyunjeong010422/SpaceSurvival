using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileManager4 : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] profileCards;
    [SerializeField] TMP_Text[] nickNameTexts;
    [SerializeField] Image[] profileImages;
    [SerializeField] TMP_Text[] scoreTexts;
    [SerializeField] TMP_Text[] hpTexts;
    [SerializeField] Color myProfileColor = default; // 내 프로필 카드 색상

    private void Awake()
    {
        // Hex 색상을 Color로 변환
        if (ColorUtility.TryParseHtmlString("#FF3636", out var color))
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
        TPSPlayerController4 playerController = PhotonNetwork.LocalPlayer.TagObject as TPSPlayerController4;
        if (playerController != null)
        {
            UpdateProfileInfo(PhotonNetwork.LocalPlayer.ActorNumber - 1, playerController.GetScore(), playerController.GetHealth());
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
        InitializeProfileCards();
    }

    // 플레이어 수에 맞게 프로필 카드 활성화
    private void InitializeProfileCards()
    {
        int playerCount = PhotonNetwork.PlayerList.Length; // 현재 방에 있는 플레이어 수

        // 플레이어 수에 맞게 카드 활성화 및 비활성화
        for (int i = 0; i < 4; i++)
        {
            profileCards[i].SetActive(i < playerCount);

            if (i < playerCount)
            {
                // 각 플레이어의 닉네임 가져오기
                nickNameTexts[i].text = PhotonNetwork.PlayerList[i].NickName;

                if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
                {
                    profileCards[i].GetComponent<Image>().color = myProfileColor; // 본인 프로필 카드 색상 변경
                    TPSPlayerController4 playerController = PhotonNetwork.LocalPlayer.TagObject as TPSPlayerController4;
                    if (playerController != null)
                    {
                        // 내 점수와 HP를 업데이트
                        UpdateProfileInfo(i, playerController.GetScore(), playerController.GetHealth());
                    }
                }
            }
        }
    }

    // 프로필 정보 업데이트
    public void UpdateProfileInfo(int playerIndex, int score, int hp)
    {
        // 해당 플레이어의 점수와 HP 업데이트
        scoreTexts[playerIndex].text = $"{score}";
        hpTexts[playerIndex].text = (playerIndex == PhotonNetwork.LocalPlayer.ActorNumber - 1) ? $"HP: {hp}" : " "; // 본인만 HP 표시
    }
}
