using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IcySlidingGameScene : MiniGameSceneBase
{
    [SerializeField] PlayerInfoPanel2 playerInfoUI;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] Transform icePlatform;
    [SerializeField] LocalPlayerTrigger2 failTrigger;

    // 승점 UI
    [SerializeField] RectTransform winningScoreUI;

    [Header("게임 설정")]
    [SerializeField] float maxPlayTime = 20f; // 플랫폼 크기가 0이 되는 시간

    private PlayerCharacterControl2 localPlayerCharacter;
    private Coroutine gamePlayRoutine;
    private bool[] playerIsFali; // 플레이어 추락 시점에 true, index는 GetPlayerNumber
    private int alivePlayers;

    protected override void ReadyNetworkScene()
    {
    }

    protected override void ReadyPlayerClient()
    {
        GameObject instance = PhotonNetwork.Instantiate("Character2 IcySliding", new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)), Quaternion.identity);
        localPlayerCharacter = instance.GetComponent<PlayerCharacterControl2>();

        Camera.main.GetComponent<CameraController2>().Target = localPlayerCharacter.transform;
        localPlayerCharacter.enabled = false; // 게임 시작 전까지 플레이어 컨트롤 비활성화
        alivePlayers = PhotonNetwork.PlayerList.Length;
        playerIsFali = new bool[alivePlayers];

        // UI 초기화
        playerInfoUI.InitRoomPlayerInfo("생존");

        // 탈락 트리거에 리스너 설정
        failTrigger.OnLocalPlayerTriggered.AddListener(_ => LocalPlayerFailed());
    }

    protected override void GameStart()
    {
        gamePlayRoutine = StartCoroutine(CountDownAndStart());
    }

    private IEnumerator CountDownAndStart()
    {
        YieldInstruction waitCountDown = new WaitForSeconds(1f);
        YieldInstruction platformReducePeriod = new WaitForSeconds(0.1f);
        Vector3 scaleReducePerPeriod;
        {
            float reducePerPeriod = 0.1f / maxPlayTime;
            scaleReducePerPeriod = new Vector3(reducePerPeriod, 0f, reducePerPeriod);
        }

        countdownText.gameObject.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            countdownText.text = (5 - i).ToString();
            yield return waitCountDown;
        }
        countdownText.gameObject.SetActive(false);

        // 카운트다운 종료 후 입력 활성화
        localPlayerCharacter.enabled = true;

        // 플랫폼 크기 점차 감소
        while (icePlatform.localScale.x > 0)
        {
            yield return platformReducePeriod;
            icePlatform.localScale -= scaleReducePerPeriod;
        }
        icePlatform.gameObject.SetActive(false);
    }

    private void LocalPlayerFailed()
    {
        photonView.RPC(nameof(FailedRPC), RpcTarget.AllViaServer);
        Camera.main.GetComponent<CameraController2>().Target = icePlatform; // 탈락시 카메라의 플레이어 추적 중단
    }

    [PunRPC]
    private void FailedRPC(PhotonMessageInfo info)
    {
        Player failedPlayer = info.Sender;
        int playerNumber = failedPlayer.GetPlayerNumber();

        if (playerIsFali[playerNumber])
        {
            Debug.Log("한 플레이어의 탈락 메서드가 여러번 호출됨");
            return;
        }

        playerInfoUI.SetText(failedPlayer, "탈락");

        playerIsFali[playerNumber] = true;
        alivePlayers--;

        if (alivePlayers == 1)
        {
            Player winner = null;
            for (int i = 0; i < playerIsFali.Length; i++)
            {
                if (false == playerIsFali[i])
                {
                    winner = PlayerNumbering.SortedPlayers[i];
                    break;
                }
            }

            if (winner == null)
            {
                Debug.LogWarning($"승자를 찾지 못함");
                return;
            }

            Debug.Log($"승자: {winner.NickName}");

            winningScoreUI.gameObject.SetActive(true);
        }
    }
}
