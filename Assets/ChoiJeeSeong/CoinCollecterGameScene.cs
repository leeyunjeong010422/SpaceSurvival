using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCollecterGameScene : MiniGameSceneBase
{
    [SerializeField] MiniGameScore scoreManager;
    [SerializeField] PlayerInfoPanel2 playerInfoUI;
    [SerializeField] TMP_Text countdownText;

    private PlayerCharacterControl2 localPlayerCharacter;
    private Coroutine gamePlayRoutine;

    protected override void ReadyNetworkScene()
    {
        // 마스터 클라이언트만 할 작업
        // 맵에 코인 깔기
    }

    protected override void ReadyPlayerClient()
    {
        // 로컬 플레이어의 캐릭터 생성
        GameObject instance = PhotonNetwork.Instantiate("Character2", new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)), Quaternion.identity);
        localPlayerCharacter = instance.GetComponent<PlayerCharacterControl2>();

        localPlayerCharacter.enabled = false; // 게임 시작 전까지 플레이어 컨트롤 비활성화

        // 스코어 시스템 및 UI 초기화
        playerInfoUI.InitRoomPlayerInfo();
        scoreManager.InitScoreTable();
        scoreManager.OnScoreChanged.AddListener(playerInfoUI.SetScore);
    }

    protected override void GameStart()
    {
        // 모든 플레이어의 로딩이 완료된 시점
        // 게임 타이머 시작
        gamePlayRoutine = StartCoroutine(GamePlayRoutine());
    }

    private IEnumerator GamePlayRoutine()
    {
        YieldInstruction waitCountDown = new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            countdownText.text = (5 - i).ToString();
            yield return waitCountDown;
        }
        countdownText.gameObject.SetActive(false);

        // 카운트다운 종료 후 입력 활성화
        localPlayerCharacter.enabled = true;
    }
}
