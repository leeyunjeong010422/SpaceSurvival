using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainPanel1 : MonoBehaviour
{
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);
        roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
        maxPlayerInputField.text = "8";
    }

    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        if (roomName == "") return;

        int maxPlayer = int.Parse(maxPlayerInputField.text);
        maxPlayer = Mathf.Clamp(maxPlayer, 2, 4);

        RoomOptions roomOptions = new();
        roomOptions.MaxPlayers = maxPlayer;

        createRoomPanel.SetActive(false);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    public void RandomMatching()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }
}
