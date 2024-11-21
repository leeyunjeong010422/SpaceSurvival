using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AISpawner3 : MonoBehaviourPunCallbacks
{
    [Header("AI 스폰 설정")]
    [SerializeField] GameObject AIPrefab;
    [Range(1, 50)]
    public int AICount;
    public float spawnAreaSize;

    private HashSet<Vector3> validPosition = new HashSet<Vector3>(); // 중복 생성을 피하기 위함

    void Start()
    {
        // 마스터 클라이언트만 AI 소환
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnAI(AICount);
        }
    }

    void SpawnAI(int count)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            // PhotonNetwork.Instantiate를 사용하여 네트워크에서 AI를 생성
            PhotonNetwork.Instantiate(AIPrefab.name, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool validPosition = false; // 위치가 유효한지
        int attempts = 0;

        while (!validPosition && attempts < 5)
        {
            float randomX = Random.Range(-spawnAreaSize, spawnAreaSize);
            float randomZ = Random.Range(-spawnAreaSize, spawnAreaSize);
            randomPosition = new Vector3(randomX, 1f, randomZ);

            if (!this.validPosition.Contains(randomPosition))
            {
                this.validPosition.Add(randomPosition);
                validPosition = true;
            }

            attempts++;
        }

        return validPosition ? randomPosition : Vector3.zero;
    }

    // 클라이언트가 네트워크로부터 동기화된 AI 정보를 받을 때, AI의 상태를 업데이트하는 메서드 (예시)
    public override void OnJoinedRoom()
    {
        // 이 메서드는 모든 클라이언트에서 호출됩니다.
        if (!PhotonNetwork.IsMasterClient)
        {
            // AI 생성과 관련된 작업을 마스터 클라이언트가 처리하도록 하므로
            // AI가 생성된 후, 각 클라이언트는 PhotonNetwork.Instantiate로 생성된 AI를 자동으로 확인하게 됩니다.
            Debug.Log("AI가 생성되었습니다.");
        }
    }
}
