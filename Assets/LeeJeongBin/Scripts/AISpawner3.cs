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

    private HashSet<Vector3> validPosition = new HashSet<Vector3>(); // 유효한 위치를 저장

    void Start()
    {
        // AI 생성은 마스터 클라이언트만 담당
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnAI(AICount);
        }
    }

    // AI 캐릭터 생성 담당
    public void SpawnAI(int count)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 지정된 수만큼 AI 생성
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(); // 랜덤 위치
            if (spawnPosition != Vector3.zero) // 유효한 위치일 경우에
            {
                // AI 캐릭터 인스턴스 생성 및 동기화
                GameObject aiObject = PhotonNetwork.Instantiate(AIPrefab.name, spawnPosition, Quaternion.identity);

                // AI 캐릭터 소유권은 마스터 클라이언트
                PhotonView aiPhotonView = aiObject.GetComponent<PhotonView>();
                if (aiPhotonView != null && !aiPhotonView.IsMine)
                {
                    aiPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer); // 소유권을 로컬 플레이어에게 넘김
                }
            }
        }
    }

    // AI가 생성될 랜덤 위치 계산
    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool validPosition = false; // 유효한 위치인지 체크
        int attempts = 0; // 시도 횟수 변수

        while (!validPosition && attempts < 5) // 5번까지 시도
        {
            // 랜덤 좌표 생성
            float randomX = Random.Range(-spawnAreaSize, spawnAreaSize);
            float randomZ = Random.Range(-spawnAreaSize, spawnAreaSize);
            randomPosition = new Vector3(randomX, 0f, randomZ);

            validPosition = true;

            // 이미 생성된 AI와의 충돌을 방지
            foreach (Vector3 pos in this.validPosition)
            {
                if (Vector3.Distance(randomPosition, pos) < 5f) // 해당 거리 이내에 이미 AI가 있으면 생성x
                {
                    validPosition = false;
                    break;
                }
            }

            attempts++;
        }

        if (validPosition)
        {
            this.validPosition.Add(randomPosition); // 유효한 위치를 저장하여
            return randomPosition; // 유효한 위치를 반환
        }

        return Vector3.zero; // 실패 시 (Vector3.zero 반환)
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("AI가 생성되었습니다.");
        }
    }
}
