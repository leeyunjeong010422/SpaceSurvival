using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAndSeekGameScene : MiniGameSceneBase
{
    [Header("AI 스포너 참조")]
    [SerializeField] AISpawner3 aiSpawner;

    [Header("체크포인트 오브젝트들")]
    [SerializeField] private List<Transform> checkpointTransforms;  // 체크포인트 오브젝트들의 위치를 담을 리스트

    [Header("체크포인트 주변 반경")]
    [SerializeField] private float checkpointRadius = 10f;  // 체크포인트 주변에서 제외할 반경

    [Header("최대 시도 횟수")]
    [SerializeField] private int maxSpawnAttempts = 10;  // 스폰 위치를 찾기 위한 최대 시도 횟수

    protected override void GameStart()
    {
    }

    protected override void ReadyNetworkScene()
    {
        // 마스터 클라이언트에서 AI 생성
        aiSpawner.SpawnAI(aiSpawner.AICount);
    }

    protected override void ReadyPlayerClient()
    {
        // 플레이어 생성 위치를 랜덤으로
        Vector3 playerSpawnPosition = GetRandomSpawnPosition(aiSpawner.spawnAreaSize);
        PhotonNetwork.Instantiate("Player", playerSpawnPosition, Quaternion.identity);
    }

    // 플레이어 스폰 위치를 체크포인트 반경을 피해서 랜덤으로 생성
    private Vector3 GetRandomSpawnPosition(float spawnAreaSize)
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
            spawnPosition = new Vector3(randomX, 0f, randomZ); // y 값은 1로 고정

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

    private bool IsSpawnPositionValid(Vector3 position)
    {
        foreach (Transform checkpoint in checkpointTransforms)
        {
            // 체크포인트로부터의 거리 계산
            float distance = Vector3.Distance(position, checkpoint.position);

            // 체크포인트 반경 내에 있다면 옳지 않은 위치
            if (distance < checkpointRadius)
            {
                return false;
            }
        }

        // 모든 체크포인트 주변에 없다면 유효한 위치
        return true;
    }

}
