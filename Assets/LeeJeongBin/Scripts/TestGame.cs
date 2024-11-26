using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TestGame : MonoBehaviourPunCallbacks
{
    [Header("AI 스포너 참조")]
    [SerializeField] AISpawner3 aiSpawner;

    [Header("체크포인트 오브젝트들")]
    [SerializeField] private List<Transform> checkpointTransforms;  // 체크포인트 오브젝트들의 위치를 담을 리스트

    [Header("체크포인트 주변 반경")]
    [SerializeField] private float checkpointRadius = 10f;  // 체크포인트 주변에서 제외할 반경

    [Header("최대 시도 횟수")]
    [SerializeField] private int maxSpawnAttempts = 10;  // 스폰 위치를 찾기 위한 최대 시도 횟수

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

        // 플레이어 생성 위치를 랜덤으로
        Vector3 playerSpawnPosition = GetRandomSpawnPosition(aiSpawner.spawnAreaSize);
        PhotonNetwork.Instantiate("Player", playerSpawnPosition, Quaternion.identity);

        // 마스터 클라이언트에서 AI 생성
        if (PhotonNetwork.IsMasterClient && aiSpawner != null)
        {
            aiSpawner.SpawnAI(aiSpawner.AICount);
        }
    }

    // 플레이어 스폰 위치를 체크포인트 반경을 피해서 랜덤으로 생성
    Vector3 GetRandomSpawnPosition(float spawnAreaSize)
    {
        Vector3 spawnPosition = Vector3.zero;
        bool validPosition = false;
        int attempts = 0;  // 시도 횟수 변수

        // 적절한 스폰 위치가 나올 때까지
        while (!validPosition && attempts < maxSpawnAttempts)
        {
            // 스폰 범위 내에서 랜덤으로 생성
            float randomX = Random.Range(-spawnAreaSize, spawnAreaSize);
            float randomZ = Random.Range(-spawnAreaSize, spawnAreaSize);
            spawnPosition = new Vector3(randomX, 1f, randomZ); // y 값은 1로 고정

            // 체크포인트 범위 내에 있는지 확인
            validPosition = IsSpawnPositionValid(spawnPosition);

            attempts++;
        }

        // 최대 시도 횟수를 넘겼을 경우에도 실패 했을 경우
        if (!validPosition)
        {
            // 기본 위치로 스폰
            spawnPosition = new Vector3(0f, 1f, 0f);
        }

        return spawnPosition;
    }

    bool IsSpawnPositionValid(Vector3 position)
    {
        foreach (Transform checkpoint in checkpointTransforms)
        {
            // 체크포인트로부터의 거리 계산
            float distance = Vector3.Distance(position, checkpoint.position);

            // 체크포인트 반경 내에 있다면 옳지 안흔 위치
            if (distance < checkpointRadius)
            {
                return false;
            }
        }

        // 모든 체크포인트 주변에 없다면 유효한 위치
        return true;
    }
}
