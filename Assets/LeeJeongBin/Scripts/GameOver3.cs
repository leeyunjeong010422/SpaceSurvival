using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameOver3 : MonoBehaviourPun
{
    public static GameOver3 Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 플레이어 사망 시 호출
    public void OnPlayerDeath(string playerName)
    {
        // 네트워크 상 존재하는 다른 플레이어들이 모두 사망한 경우 승리
        if (PhotonNetwork.PlayerListOthers.Length == 0)
        {
            PlayerWin(playerName);
        }
    }

    // 승리 처리
    public void PlayerWin(string winnerName)
    {
        Debug.Log($"{winnerName} 승리");

        if (photonView == null)
            return;

        // 모든 클라이언트에게 승리 메시지 전달
        photonView.RPC("GameWinner", RpcTarget.All, winnerName);
    }

    [PunRPC]
    private void GameWinner(string winnerName)
    {
        // 게임 종료 처리 및 승리 메시지 출력
        Debug.Log($"게임 종료 {winnerName}이 승리했습니다");

        // 여기서 게임 종료 후 UI 처리 및 기타 종료 로직 추가 가능
        EndGame();
    }

    private void EndGame()
    {
        // 게임 종료 후 UI 활성화, 리소스 해제 등 추가 처리 가능
        // UIManager.Instance.ShowEndGameScreen(winnerName);
        Debug.Log("게임이 종료되었습니다. 종료 UI 및 리소스 정리 필요");
    }
}
