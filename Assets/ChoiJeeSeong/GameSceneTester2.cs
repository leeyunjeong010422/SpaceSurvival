using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class GameSceneTester2 : MonoBehaviourPunCallbacks
{
    [SerializeField] int maxPlayers = 4;
    [SerializeField] MiniGameSceneBase sceneManager;

    [Header("참여 인원 정보")]
    [SerializeField] List<string> nickNames;

    private string testRoomName;

    private void Awake()
    {
        // 서로 다른 씬을 테스트하는 상황에서 충돌이 나지 않도록 방 이름 구분
        testRoomName = $"TestRoom {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}";

        // 로비를 통해 들어와서 이미 연결되어 있을 경우 사용하지 않는다
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("로비를 통해 접근해서 GameSceneTester 제거");
            sceneManager.enabled = true;
            Destroy(this);
        }
        else
        {
            if (sceneManager.enabled)
            {
                Debug.LogWarning("테스터를 사용할 경우 sceneManager.enabled를 false로 해주세요");
            }
            nickNames.Clear();
        }
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    #region PunCallbacks
    public override void OnConnectedToMaster()
    {
        // rgb에 랜덤값 넣기
        PhotonNetwork.LocalPlayer.SetColorNumber(Random.Range(0, 8));

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
        foreach (Player roomPlayer in PhotonNetwork.PlayerList)
        {
            nickNames.Add(roomPlayer.NickName);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"테스트룸에 {newPlayer.NickName} 진입");
        nickNames.Add(newPlayer.NickName);
    }
    #endregion PunCallbacks

    [ContextMenu("Test Start")]
    public void TestStart()
    {
        photonView.RPC(nameof(TestStartRPC), RpcTarget.All);
    }

    [PunRPC]
    private void TestStartRPC()
    {
        sceneManager.enabled = true;
    }

}
