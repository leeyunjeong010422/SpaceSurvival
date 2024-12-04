using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CoinCollecterGameScene : MiniGameSceneBase
{
    [SerializeField] MiniGameScore scoreManager;
    [SerializeField] PlayerInfoPanel2 playerInfoUI;
    [SerializeField] CountdownText countdownText;
    [SerializeField] Light mainLight; // 타임아웃 요소: 조명이 점점 어두워져서 Emission을 갖는 코인이 강조됨
    [SerializeField] Renderer skyDomeUpper;
    [SerializeField] Renderer skyDomeLower;

    // 승점 UI
    [SerializeField] RectTransform winningScoreUI;

    [Header("게임 설정")]
    [SerializeField] float maxPlayTime = 40f; // 조명값이 최소가 되는 시간

    /*
    애셋 선정 전 단계에서는 [SerializeField]로 맵에 배치된 코인을 참조해 초기화한다
    사용할 맵이 확정된 후 코인 생성 방식을 다시 결정
     */
    [SerializeField] LocalPlayerTrigger2[] coins;
    private int remainCoins;

    private PlayerCharacterControl2 localPlayerCharacter;
    private Coroutine gamePlayRoutine;
    private Material skyDomeMaterial;

    protected override void ReadyNetworkScene()
    {
        // 마스터 클라이언트만 할 작업
        // 코인 무작위 생성?
    }

    protected override void ReadyPlayerClient()
    {
        // 로컬 플레이어의 캐릭터 생성
        NavMeshHit spawnPoseHit;
        while (false == NavMesh.SamplePosition(new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f)), out spawnPoseHit, 3f, NavMesh.AllAreas));

        GameObject instance = PhotonNetwork.Instantiate("Character2", spawnPoseHit.position, Quaternion.identity);
        localPlayerCharacter = instance.GetComponent<PlayerCharacterControl2>();

        Camera.main.GetComponent<CameraController2>().Target = localPlayerCharacter.transform;
        localPlayerCharacter.enabled = false; // 게임 시작 전까지 플레이어 컨트롤 비활성화

        // 스코어 시스템 및 UI 초기화
        playerInfoUI.InitRoomPlayerInfo();
        scoreManager.InitScoreTable();
        scoreManager.OnScoreChanged.AddListener(playerInfoUI.SetInt);

        // 코인 초기화
        remainCoins = coins.Length;
        for (int i = 0; i < coins.Length; i++)
        {
            coins[i].Id = i;
            coins[i].OnLocalPlayerTriggered.AddListener(TryGetCoin);
        }

        // 하늘 색상 설정 준비
        skyDomeUpper.material.mainTextureOffset = new Vector2(0f, 0f);
        skyDomeMaterial = skyDomeLower.material = skyDomeUpper.material;
    }

    protected override void GameStart()
    {
        // 모든 플레이어의 로딩이 완료된 시점
        // 게임 타이머 시작
        countdownText.CountdownStart(5);
        countdownText.OnCountdownComplete.AddListener(GamePlay);
    }

    private void GamePlay()
    {
        gamePlayRoutine = StartCoroutine(GamePlayRoutine());
    }

    private IEnumerator GamePlayRoutine()
    {
        YieldInstruction lightReducePeriod = new WaitForSeconds(0.1f);
        float lightReducePerPeriod = 0.1f / maxPlayTime;

        // 카운트다운 종료 후 입력 활성화
        localPlayerCharacter.enabled = true;

        while (mainLight.intensity > 0f)
        {
            yield return lightReducePeriod;
            mainLight.intensity -= lightReducePerPeriod; // 메인 방향 조명 어둡게
            skyDomeMaterial.mainTextureOffset = new Vector2(0.5f * (1f - mainLight.intensity), 0); // 스카이돔 밝기 설정
        }

        mainLight.intensity = 0f;
    }

    private void TryGetCoin(int coinId)
    {
        // 호출 순서가 일치해야 하므로 ViaServer를 사용
        photonView.RPC(nameof(TryGetCoinRPC), RpcTarget.AllViaServer, coinId);
    }

    [PunRPC]
    private void TryGetCoinRPC(int coinId, PhotonMessageInfo info)
    {
        // 네트워크 딜레이 예외처리
        if (false == coins[coinId].isActiveAndEnabled)
        {
            Debug.Log("이미 다른 플레이어가 습득한 코인");
            return;
        }

        // 자신이 전송한 RPC일 경우 점수 획득
        if (info.Sender.IsLocal)
        {
            scoreManager.AddScore(10);
        }

        coins[coinId].gameObject.SetActive(false);
        remainCoins--;

        Debug.Log($"남은 코인: {remainCoins}개");

        if (remainCoins <= 0)
        {
            StopCoroutine(gamePlayRoutine);
            gamePlayRoutine = StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);

        // 모든 게임이 수집되어 미니게임 종료
        // 승자 결정 및 승점 UI 띄우기
        // 승점 UI에서 다음 스테이지로 이동하기 위해 READY

        int winnerScore = scoreManager.ScoreTable.Max(x => x.Value);

        if (winnerScore == scoreManager.ScoreTable[PhotonNetwork.LocalPlayer.ActorNumber])
        {
            // 최고점 혹은 최고점과 동점이라면 승리
            PhotonNetwork.LocalPlayer.SetWinningPoint(10 + PhotonNetwork.LocalPlayer.GetWinningPoint()); // 승점 획득
        }

        winningScoreUI.gameObject.SetActive(true);
    }
}
