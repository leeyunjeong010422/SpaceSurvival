using Photon.Pun;
using UnityEngine;

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

    public void OnPlayerDeath(string playerName)
    {
        Debug.Log($"{playerName}이 사망했습니다.");

        if (playerName.StartsWith("AI"))
        {
            Debug.Log("AI는 승리 조건에 포함되지 않습니다.");
            return;
        }

        if (PhotonNetwork.PlayerListOthers.Length == 0)
        {
            PlayerWin(playerName);
        }
    }

    public void PlayerWin(string winnerName)
    {
        Debug.Log($"{winnerName} 승리!");

        if (photonView == null)
        {
            Debug.LogError("PhotonView가 null입니다! RPC 호출을 할 수 없습니다.");
            return;
        }

        photonView.RPC("GameWinner", RpcTarget.All, winnerName);
    }

    [PunRPC]
    private void GameWinner(string winnerName)
    {
        Debug.Log($"게임 종료! {winnerName}이 승리했습니다!");
        // 게임 종료 처리
    }
}