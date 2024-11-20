using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner3 : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject AIPrefab;
    [Range(1, 50)]
    public int AICount;
    public float spawnAreaSize;

    private HashSet<Vector3> validPosition = new HashSet<Vector3>(); // 중복 생성을 피하기 위함

    void Start()
    {
        SpawnAI(AICount);
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
            GameObject newAI = Instantiate(AIPrefab, spawnPosition, Quaternion.identity);

            AIController3 aiController = newAI.GetComponent<AIController3>();
            aiController.moveSpeed = 5f;
            aiController.minPauseTime = 1f;
            aiController.maxPauseTime = 3f;
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
}
