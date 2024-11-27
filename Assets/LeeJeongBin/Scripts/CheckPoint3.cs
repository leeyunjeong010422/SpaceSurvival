using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint3 : MonoBehaviourPun
{
    public static CheckPoint3 Instance;
    public int TotalCheckPoints { get; private set; } = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController3 playerController = other.GetComponent<PlayerController3>();

            if (playerController != null && !PlayerController3.visitedCheckPoint.Contains(this))
            {
                Debug.Log($"플레이어가 체크포인트 {gameObject.name}에 도달했습니다.");
                playerController.OnTriggerCheckPoint(this);

                Debug.Log($"현재 체크포인트 통과 : {playerController.CheckPointsReached}");

                if (playerController.CheckPointsReached >= TotalCheckPoints)
                {
                    // 모든 체크포인트를 통과한 경우
                    Debug.Log("모든 체크포인트를 통과하여 승리하였습니다.");
                    // 게임 완료 또는 승리 로직 추가 해야함
                }
            }
        }
    }
}
