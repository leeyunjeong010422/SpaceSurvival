using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TPSGameSceneTest4 : MonoBehaviourPunCallbacks
{
    private float gameTimer; // 게임 시간
    [SerializeField] private TMP_Text timerText;
    private bool gameStarted = false;
    public bool isGameEnded = false;
    private GameObject endGamePanel;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoad(true);
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings(); // Photon 서버 연결
    }

    public override void OnConnectedToMaster()
    {
        //테스트 할 때 맥스플레이어를 2명으로 설정 (밑에서 다 로딩이 되는 경우를 테스트 해야함)
        RoomOptions options = new RoomOptions { MaxPlayers = 2, IsVisible = false }; // 비공개방 설정
        PhotonNetwork.JoinOrCreateRoom($"TestRoom {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}", options, null);
    }

    public override void OnJoinedRoom()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-30, 30), 0.5f, Random.Range(-30, 30));
        PhotonNetwork.Instantiate("TPS_Player4", spawnPosition, Quaternion.identity);

        endGamePanel = GameObject.Find("Canvas/EndGamePanel");
        endGamePanel?.SetActive(false);

        // 플레이어 로딩 및 입장 확인 시작
        StartCoroutine(CheckAllPlayersLoadedAndStartGame());
    }

    private IEnumerator CheckAllPlayersLoadedAndStartGame()
    {
        // 모든 플레이어가 입장하고 로딩이 완료될 때까지 대기
        while (!IsRoomReady())
        {
            Debug.Log("다른 플레이어 로딩 대기 중");
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("모두 로딩 완료");
        GameStart();
    }

    private bool IsRoomReady()
    {
        // 현재 룸에 모든 플레이어가 입장했는지 확인
        if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            return false;
        }

        // 모든 플레이어의 로딩 상태 확인
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.GetLoad()) return false;
        }

        return true;
    }

    private void Update()
    {
        if (!gameStarted) return;

        // 게임 타이머
        gameTimer -= Time.deltaTime;
        timerText.text = $"Time: {Mathf.CeilToInt(gameTimer)}";

        if (gameTimer <= 0) EndGame();
    }

    private void GameStart()
    {
        StartCoroutine(CountDownRoutine());
    }

    IEnumerator CountDownRoutine()
    {
        for (int i = 3; i > 0; i--)
        {
            if (timerText != null)
            {
                timerText.text = i.ToString();
            }
        }

        if (timerText != null)
        {
            timerText.text = "START!";
        }
        yield return new WaitForSeconds(1f);

        // 3초 카운트다운 후 게임 시작
        gameStarted = true;
        StartGameTimer();
    }

    private void StartGameTimer()
    {
        Debug.Log("타이머 시작한다");
        gameTimer = 30f;
    }

    private void EndGame()
    {
        Debug.Log("게임 끝");
        gameStarted = false;
        isGameEnded = true;
        endGamePanel?.SetActive(true);
        DisplayRankings();
    }

    private void DisplayRankings()
    {
        Debug.Log("랭크 표시");
        TPSPlayerController4[] players = FindObjectsOfType<TPSPlayerController4>();
        var playerScores = new List<(string playerName, int score, bool isLocalPlayer)> ();

        foreach (var player in players)
        {
            bool isLocal = player.photonView.IsMine; // 내 플레이어 여부 확인
            playerScores.Add((player.photonView.Owner.NickName, player.GetScore(), isLocal));
        }

        int maxScore = -1;

        foreach (var scoreData in playerScores)
        {
            if (scoreData.score > maxScore)
            {
                maxScore = scoreData.score;
            }
        }

        var topScorers = playerScores.FindAll(player => player.score == maxScore);

        var rankingText = endGamePanel.GetComponentsInChildren<TMP_Text>();
        if (rankingText.Length > 0)
        {
            string topScorerText = "";

            foreach (var scorer in topScorers)
            {
                string playerName = scorer.playerName;
                int score = scorer.score;
                if (scorer.isLocalPlayer)
                {
                    topScorerText += $"<color=red>닉네임: {playerName} / 점수: {score}</color>\n";
                }
                else
                {
                    topScorerText += $"닉네임: {playerName} / 점수: {score}\n";
                }
            }
            rankingText[0].text = topScorerText.TrimEnd('\n');
        }
    }
    private Vector3 RandomPositionNavMesh(Vector3 center, float range)
    {
        // 임의의 위치 생성
        Vector3 randomPosition = center + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

        // NavMesh에서 유효한 위치인지 확인
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, range, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }
}
