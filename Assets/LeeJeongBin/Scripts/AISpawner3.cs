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

    private HashSet<Vector3> validPosition = new HashSet<Vector3>();

    void Start()
    {
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
            PhotonNetwork.Instantiate(AIPrefab.name, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool validPosition = false;
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

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("AI가 생성되었습니다.");
        }
    }
}
