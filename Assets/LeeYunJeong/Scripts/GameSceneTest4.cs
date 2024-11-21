using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameSceneTest4 : MonoBehaviourPunCallbacks
{
    private string testRoomName;

    private void Start()
    {
        testRoomName = $"TestRoom {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}";
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings(); // Photon 서버 연결
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = false; // 비공개방으로 설정

        PhotonNetwork.JoinOrCreateRoom(testRoomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10)); // 임의의 생성 위치
        PhotonNetwork.Instantiate("Player4", spawnPosition, Quaternion.identity);

    }
}
