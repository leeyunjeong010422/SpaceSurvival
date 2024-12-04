using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry1 : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] Button joinRoomButton;

    // Room 정보를 입력하는 함수
    public void SetRoomInfo(RoomInfo Info)
    {
        roomName.text = Info.Name;
        currentPlayer.text = $"{Info.PlayerCount} / {Info.MaxPlayers}";
        joinRoomButton.interactable = Info.PlayerCount < Info.MaxPlayers;
    }
    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName.text);
        PopUp1.Instance.PopUpOpen(true, "방에 들어가고 있어요");
    }
}

