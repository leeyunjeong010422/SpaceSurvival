using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneTester2 : MonoBehaviourPunCallbacks
{
    [SerializeField] string testRoomName = "TestRoom";
    [SerializeField] int maxPlayers = 8;
    [SerializeField] GameObject sceneManager;

    [Header("참여 인원 정보")]
    [SerializeField] List<string> nickNames;

    private void Awake()
    {
        // 로비를 통해 들어와서 이미 연결되어 있을 경우 사용하지 않는다
        if (PhotonNetwork.IsConnected)
        {
            Destroy(this);
        }
        else
        {
            // 테스터를 사용할 경우 씬 관리자 대기
            sceneManager.SetActive(false);
            nickNames.Clear();
        }
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(testRoomName,
            new RoomOptions()
            {
                IsVisible = false,
                MaxPlayers = maxPlayers,
            },
            TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("테스트룸 생성");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("테스트룸 참가");
        nickNames.Add(PhotonNetwork.LocalPlayer.NickName);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"테스트룸에 {newPlayer.NickName} 진입");
        nickNames.Add(newPlayer.NickName);
    }

    [ContextMenu("Test Start")]
    private void TestStart()
    {
        sceneManager.SetActive(true);
    }
}
