using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint3 : MonoBehaviour
{
    public static CheckPoint3 Instance;
    public int TotalCheckPoints { get; private set; } = 4;
    public static List<CheckPoint3> visitedCheckPoints = new List<CheckPoint3>();

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
            if (playerController != null && !visitedCheckPoints.Contains(this))
            {
                Debug.Log($"플레이어가 체크포인트 {gameObject.name}에 도달했습니다");
                playerController.OnTriggerCheckPoint(this);
                visitedCheckPoints.Add(this);
            }
        }
    }

    public static void ResetCheckPoints()
    {
        visitedCheckPoints.Clear();
    }
}
