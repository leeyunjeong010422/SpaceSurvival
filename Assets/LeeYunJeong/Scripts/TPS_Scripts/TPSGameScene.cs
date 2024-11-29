using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TPSGameScene : MiniGameSceneBase
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject winninPointPanel;
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] AudioClip TPSBGM;

    private float gameTimer;
    private bool gameStarted = false;
    private List<(string playerName, int score, bool isLocalPlayer)> playerScores;

    protected override void ReadyNetworkScene()
    {
        // 마스터 클라이언트가 초기화할 작업
    }

    protected override void ReadyPlayerClient()
    {
        // 로컬 플레이어의 캐릭터 생성
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject playerInstance = PhotonNetwork.Instantiate("TPS_Player4", spawnPosition, Quaternion.identity);

        // UI 초기화
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
    }

    protected override void GameStart()
    {
        // 모든 플레이어 로딩 완료 후 게임 시작
        StartCoroutine(GameStartRoutine());
        GameManager.Sound.PlayBGM(TPSBGM, 0.8f);
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
            StartCoroutine(EndGameRoutine());
        }
    }

    private IEnumerator EndGameRoutine()
    {
        Debug.Log("게임 종료");
        gameStarted = false;

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }

        DisplayRankings();

        yield return new WaitForSeconds(3f);

        winninPointPanel.SetActive(true);

        GameManager.Sound.StopBGM();
    }

    private void DisplayRankings()
    {
        // 모든 플레이어 점수를 수집
        var players = FindObjectsOfType<TPSPlayerController4>();
        playerScores = new List<(string playerName, int score, bool isLocalPlayer)>();

        foreach (var player in players)
        {
            bool isLocal = player.photonView.IsMine; // 본인 여부 확인
            playerScores.Add((player.photonView.Owner.NickName, player.GetScore(), isLocal));
        }

        // 최고 점수 계산
        int maxScore = -1;
        foreach (var scoreData in playerScores)
        {
            if (scoreData.score > maxScore)
            {
                maxScore = scoreData.score;
            }
        }

        // 최고 점수와 동일한 플레이어들을 필터링
        var topScorers = playerScores.FindAll(player => player.score == maxScore);

        // UI에 표시
        var rankingText = endGamePanel.GetComponentsInChildren<TMP_Text>();
        if (rankingText.Length > 0)
        {
            string rankingDisplay = "";

            foreach (var scorer in topScorers)
            {
                string playerName = scorer.playerName;
                int score = scorer.score;
                if (scorer.isLocalPlayer)
                {
                    rankingDisplay += $"<color=red>닉네임: {playerName} / 점수: {score}</color>\n";

                    // 로컬 플레이어에 승점 추가
                    PhotonNetwork.LocalPlayer.SetWinningPoint(10 + PhotonNetwork.LocalPlayer.GetWinningPoint());
                }
                else
                {
                    rankingDisplay += $"닉네임: {playerName} / 점수: {score}\n";
                }
            }

            rankingText[0].text = rankingDisplay.TrimEnd('\n');
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-30, 30), 0.5f, Random.Range(-30, 30));
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return randomPosition;
    }
}
