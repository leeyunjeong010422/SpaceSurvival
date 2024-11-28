using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TtangttameokgiGameScene : MiniGameSceneBase
{
    [SerializeField] private TMP_Text timerText;
    private GameObject endGamePanel;
    [SerializeField] GameObject winningPointPanel;
    [SerializeField] private float gameDuration = 30f;

    private float gameTimer; // 게임 시간
    public bool gameStarted = false;

    protected override void ReadyNetworkScene()
    {
        // 마스터 클라이언트가 초기화할 작업
    }

    protected override void ReadyPlayerClient()
    {
        Vector3 spawnPosition = RandomPositionNavMesh(Vector3.zero, 10f);
        GameObject playerObject = PhotonNetwork.Instantiate("TTMG_Player4", spawnPosition, Quaternion.identity);

        PlayerController4 playerController = playerObject.GetComponent<PlayerController4>();
        PhotonNetwork.LocalPlayer.TagObject = playerController;

        endGamePanel = GameObject.Find("Canvas/EndGamePanel");
        endGamePanel?.SetActive(false);
    }

    protected override void GameStart()
    {
        StartCoroutine(GameStartRoutine());
    }

    private IEnumerator GameStartRoutine()
    {
        // 3초 카운트다운
        for (int i = 3; i > 0; i--)
        {
            if (timerText != null)
            {
                timerText.text = i.ToString();
            }
            yield return new WaitForSeconds(1f);
        }

        if (timerText != null)
        {
            timerText.text = "START!";
        }

        yield return new WaitForSeconds(1f);

        // 게임 시작
        gameStarted = true;
        StartGameTimer();
    }

    private void StartGameTimer()
    {
        Debug.Log("게임 타이머 시작");
        gameTimer = gameDuration;
    }

    private void Update()
    {
        if (!gameStarted) return;

        // 게임 타이머 감소
        gameTimer -= Time.deltaTime;
        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(gameTimer)}";
        }

        // 타이머가 끝났을 경우 게임 종료
        if (gameTimer <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("게임 종료");
        gameStarted = false;

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }

        DisplayRankings();
    }

    private void DisplayRankings()
    {
        Player highestPlayer = null;
        int highestScore = -1;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerController4 controller = player.TagObject as PlayerController4;
            if (controller != null)
            {
                // 최고 점수 찾기
                if (controller.playerScore > highestScore)
                {
                    highestScore = controller.playerScore;
                    highestPlayer = player;
                }
            }
        }

        // 최고 점수 플레이어 출력
        if (highestPlayer != null)
        {
            string rankingText = $"<color=red>닉네임: {highestPlayer.NickName} / 점수: {highestScore}점</color>";

            TMP_Text rankingTextComponent = endGamePanel.GetComponentInChildren<TMP_Text>();
            if (rankingTextComponent != null)
            {
                rankingTextComponent.text = rankingText;
            }

            Debug.Log($"최고 점수: {highestPlayer.NickName} - {highestScore}점");

            if (highestPlayer == PhotonNetwork.LocalPlayer)
            {
                PhotonNetwork.LocalPlayer.SetWinningPoint(10 + PhotonNetwork.LocalPlayer.GetWinningPoint());
            }
            winningPointPanel.SetActive(true);
        }
    }


    private Vector3 RandomPositionNavMesh(Vector3 center, float range)
    {
        // 범위 내에서 랜덤한 위치를 생성
        Vector3 spawnPosition = center + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }
}
