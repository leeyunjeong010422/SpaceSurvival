using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePanel1 : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Menu, Lobby, Room }

    [SerializeField] LoginPanel1 loginPanel;
    [SerializeField] MainPanel1 menuPanel;
    [SerializeField] LobbyPanel1 lobbyPanel;
    [SerializeField] WaitingRoom1 waitingRoom;

    private void Start()
    {
        // 호스트의 씬을 따라가기
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.InRoom)
        {
            SetActivePanel(Panel.Room);
            StartCoroutine(waitingRoom.WinnerEventCoroutine());
            return;
        }
        // 로비씬으로 이동시 상태에 따라서 이동
        SetActivePanel(PhotonNetwork.IsConnected ? Panel.Menu : Panel.Login);

        // TPS 게임에서 visible이 꺼질 수 있어서 ture로
        Cursor.visible = true;
    }

    // 서버에 접속 했을때 호출
    public override void OnConnectedToMaster()
    {
        PopUp1.Instance.PopUpClose();
        SetActivePanel(Panel.Menu);
    }

    // 서버와 연결이 끊어질때 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(Panel.Login);
    }

    // Room 생성 실패시 호출
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Menu);
    }

    // Room 입장시 호출
    public override void OnJoinedRoom()
    {
        PopUp1.Instance.PopUpClose();

        GameObject instance = PhotonNetwork.Instantiate("Character2", Vector3.up, Quaternion.identity);

        Camera.main.GetComponent<CameraController2>().enabled = true;
        Camera.main.GetComponent<CameraController2>().Target = instance.transform;

        Cursor.lockState = CursorLockMode.Locked;

        SetActivePanel(Panel.Room);
    }

    // 플레이어가 방에서 나갈 때 호출
    public override void OnLeftRoom()
    {
        SetActivePanel(Panel.Menu);
    }
    // Room 입장에 실패할 때
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PopUp1.Instance.PopUpOpen(false, "방을 찾지 못했어요");
    }

    // 랜덤 Room 입장을 실패할 때 호출
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PopUp1.Instance.PopUpOpen(false, "방을 찾지 못했어요");
    }

    // 로비에 입장할 때 호출
    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby);
    }

    // 로비에 퇴장할 때 호출
    public override void OnLeftLobby()
    {
        lobbyPanel.ClearRoom();
        SetActivePanel(Panel.Menu);
    }

    //로비 입장 상태 && Room 상태 변경시 호출
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
        waitingRoom.gameObject.SetActive(panel == Panel.Room);
    }
}
