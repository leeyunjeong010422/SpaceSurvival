using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileManager4 : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] GameObject[] profileCards;
    [SerializeField] Image[] profileImages;
    [SerializeField] TMP_Text[] scoreTexts;
    [SerializeField] TMP_Text[] hpTexts;
    [SerializeField] Color myProfileColor = Color.red;

    private int playerHP = 100;

    private void Start()
    {
        foreach (var card in profileCards)
        {
            card.SetActive(false);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방입장");
        InitializeProfileCards();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // 새로운 플레이어가 입장할 때마다 프로필 카드 활성화
        InitializeProfileCards();
    }

    private void InitializeProfileCards()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;

        //플레이어 수에 맞게 카드 활성화
        for (int i = 0; i < 4; i++)
        {
            profileCards[i].SetActive(i < playerCount);
        }

        //본인 프로필 카드 하이라이트
        for (int i = 0; i < playerCount; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
            {
                profileCards[i].GetComponent<Image>().color = myProfileColor;
            }
        }
    }

    public void UpdateProfileInfo(int score, int hp)
    {
        //TODO: 점수, HP 관련추가
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
