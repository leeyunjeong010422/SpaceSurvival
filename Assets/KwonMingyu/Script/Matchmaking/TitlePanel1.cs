using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePanel1 : MonoBehaviourPunCallbacks
{
    public static TitlePanel1 Instance;
    public enum Panel { Login, Menu, Lobby, Room }

    [SerializeField] LoginPanel1 loginPanel;
    [SerializeField] MainPanel1 menuPanel;
    [SerializeField] LobbyPanel1 lobbyPanel;

    private void Start()
    {
        // 호스트의 씬을 따라가기
        PhotonNetwork.AutomaticallySyncScene = true;

        // 로비씬으로 이동시 상태에 따라서 이동
        SetActivePanel(PhotonNetwork.IsConnected ? Panel.Menu : Panel.Login);
    }

    // 서버에 접속 했을때 호출
    public override void OnConnectedToMaster()
    {
        SetActivePanel(Panel.Menu);
    }

    // 서버와 연결이 끊어질때 호출
    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(Panel.Login);
    }

    // Room 생성시 호출, 생성 후 생성된 방으로 이동함
    public override void OnCreatedRoom()
    {
    }

    // Room 생성 실패시 호출
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Menu);
    }

    // Room 입장시 호출
    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Cube", Vector3.zero, Quaternion.identity);
        SetActivePanel(Panel.Room);
    }

    // 다른 플레이어가 Room에 입장할 때 호출
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //roomPanel.EnterPlayer(newPlayer);
    }

    // 다른 플레이어가 Room에 퇴장할 때 호출
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //roomPanel.ExitPlayer(otherPlayer);
    }

    // 플레이어가 방에서 나갈 때 호출
    public override void OnLeftRoom()
    {
        SetActivePanel(Panel.Menu);
    }

    // 랜덤 Room 입장을 실패할 때 호출
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

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

    // 마스터 클라이언트가 변경될 때 호출
    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
    public void GameStart()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
