using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TestGame : MonoBehaviourPunCallbacks
{
    [Header("AI 스포너 참조")]
    [SerializeField] AISpawner3 aiSpawner; // AISpawner3 참조 변수

    void Start()
    {
        // Photon 네트워크에 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 네트워크에 연결되었습니다.");

        // 방을 생성하거나, 방이 이미 존재하면 해당 방에 접속
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new Photon.Realtime.RoomOptions(), Photon.Realtime.TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방에 입장했습니다.");

        // "Player" 프리팹을 인스턴스화
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

        // aiSpawner가 유효한지 확인하고, 마스터 클라이언트만 AI를 인스턴스화
        if (PhotonNetwork.IsMasterClient && aiSpawner != null)
        {
            InstantiateAI(aiSpawner.AICount, aiSpawner.spawnAreaSize);
        }
    }

    // 여러 마리 AI를 인스턴스화하는 메서드
    private void InstantiateAI(int count, float spawnAreaSize)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(spawnAreaSize); // 랜덤 위치 가져오기
            // PhotonNetwork.Instantiate를 사용하여 여러 AI 인스턴스 생성
            PhotonNetwork.Instantiate("AI", spawnPosition, Quaternion.identity);
            Debug.Log("AI #" + i + " 가 인스턴스화되었습니다.");
        }
    }

    // 랜덤 위치 생성 메서드
    Vector3 GetRandomSpawnPosition(float spawnAreaSize)
    {
        float randomX = Random.Range(-spawnAreaSize, spawnAreaSize);
        float randomZ = Random.Range(-spawnAreaSize, spawnAreaSize);
        return new Vector3(randomX, 1f, randomZ); // 높이는 1으로 설정 (플로어에 위치하게)
    }
}
