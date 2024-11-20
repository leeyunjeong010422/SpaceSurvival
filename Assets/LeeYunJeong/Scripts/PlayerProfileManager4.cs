using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileManager4 : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] profileCards;
    [SerializeField] Image[] profileImages;
    [SerializeField] TMP_Text[] scoreTexts;
    [SerializeField] TMP_Text[] hpTexts;
    [SerializeField] Color myProfileColor = Color.red; // 내 프로필 카드 색상 (인스펙터에서 변경 가능)

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
            UpdateProfileInfo(PhotonNetwork.LocalPlayer.ActorNumber - 1, playerController.GetScore(), playerController.GetHealth());
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // 플레이어가 입장할 때마다 프로필 카드 활성화
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
        }

        // 본인 프로필 카드 강조 및 프로필 정보 업데이트
        for (int i = 0; i < playerCount; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
            {
                profileCards[i].GetComponent<Image>().color = myProfileColor; // 본인 프로필 카드 색상 변경
                PlayerController4 playerController = PhotonNetwork.LocalPlayer.TagObject as PlayerController4;
                if (playerController != null)
                {
                    // 내 점수와 HP를 업데이트
                    UpdateProfileInfo(i, playerController.GetScore(), playerController.GetHealth());
                }
            }
            else
            {
                hpTexts[i].text = " "; // 다른 사람의 HP는 표시하지 않음
            }
        }
    }

    // 프로필 정보 업데이트
    public void UpdateProfileInfo(int playerIndex, int score, int hp)
    {
        // 해당 플레이어의 점수와 HP 업데이트
        scoreTexts[playerIndex].text = $"{score}";
        hpTexts[playerIndex].text = (playerIndex == PhotonNetwork.LocalPlayer.ActorNumber - 1) ? $"HP: {hp}" : ""; // 본인만 HP 표시
    }
}
