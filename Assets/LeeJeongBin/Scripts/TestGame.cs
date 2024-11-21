using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TestGame : MonoBehaviourPunCallbacks
{
    [Header("AI 스포너 참조")]
    [SerializeField] AISpawner3 aiSpawner;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new Photon.Realtime.RoomOptions(), Photon.Realtime.TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방에 입장했습니다.");

        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

        if (PhotonNetwork.IsMasterClient && aiSpawner != null)
        {
            InstantiateAI(aiSpawner.AICount, aiSpawner.spawnAreaSize);
        }
    }

    private void InstantiateAI(int count, float spawnAreaSize)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(spawnAreaSize);
            PhotonNetwork.Instantiate("AI", spawnPosition, Quaternion.identity);
            Debug.Log("AI #" + i + " 가 생성 되었습니다.");
        }
    }

    Vector3 GetRandomSpawnPosition(float spawnAreaSize)
    {
        float randomX = Random.Range(-spawnAreaSize, spawnAreaSize);
        float randomZ = Random.Range(-spawnAreaSize, spawnAreaSize);
        return new Vector3(randomX, 1f, randomZ);
    }
}
