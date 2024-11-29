using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameOver3 : MonoBehaviourPun
{
    public static GameOver3 Instance;
    [SerializeField] RectTransform winningPointUI;
    private List<Player> alivePlayers = new List<Player>(4);
    private bool gameIsOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void OnPlayerSpawn(PlayerController3 spawnedPlayerCharacter)
    {
        alivePlayers.Add(spawnedPlayerCharacter.photonView.Owner);
    }

    // 플레이어 사망 시 호출
    public void OnPlayerDeath(PlayerController3 deadPlayerCharacter)
    {
        photonView.RPC(nameof(PlayerDeathRPC), RpcTarget.AllViaServer, deadPlayerCharacter.photonView.Owner);
    }

    [PunRPC]
    private void PlayerDeathRPC(Player deadPlayer)
    {
        if (false == alivePlayers.Remove(deadPlayer))
        {
            Debug.LogWarning($"이미 탈락한 플레이어 {deadPlayer.NickName}가 다시 탈락함");
        }

        // 생존한 플레이어 캐릭터가 유일하다면 그 플레이어 승리
        if (alivePlayers.Count == 1)
        {
            PlayerWin(alivePlayers[0]);
        }
    }

    // 승리 처리
    public void PlayerWin(Player winner)
    {
        if (gameIsOver)
        {
            Debug.Log("이미 게임이 종료됨");
            return;
        }

        gameIsOver = true;

        Debug.Log($"{winner.NickName} 승리");

        if (photonView == null)
        {
            Debug.LogWarning("photonView 누락");
            return;
        }

        // 모든 클라이언트에게 승리 메시지 전달
        if (PhotonNetwork.LocalPlayer == winner)
        {
            PhotonNetwork.LocalPlayer.SetWinningPoint(10 + PhotonNetwork.LocalPlayer.GetWinningPoint());
            photonView.RPC(nameof(GameWinner), RpcTarget.All, winner);
        }
    }

    [PunRPC]
    private void GameWinner(Player winner)
    {
        gameIsOver = true;

        // 게임 종료 처리 및 승리 메시지 출력
        Debug.Log($"게임 종료 {winner.NickName}이 승리했습니다");

        // 여기서 게임 종료 후 UI 처리 및 기타 종료 로직 추가 가능
        EndGame();
    }

    private void EndGame()
    {
        // 게임 종료 후 UI 활성화, 리소스 해제 등 추가 처리 가능
        // UIManager.Instance.ShowEndGameScreen(winnerName);
        Debug.Log("게임이 종료되었습니다. 종료 UI 및 리소스 정리 필요");
        winningPointUI.gameObject.SetActive(true);
    }
}
