using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IcySlidingGameScene : MiniGameSceneBase
{
    [SerializeField] PlayerInfoPanel2 playerInfoUI;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] Transform icePlatform;

    [Header("게임 설정")]
    [SerializeField] float maxPlayTime = 20f; // 플랫폼 크기가 0이 되는 시간

    private PlayerCharacterControl2 localPlayerCharacter;
    private Coroutine gamePlayRoutine;

    protected override void ReadyNetworkScene()
    {
    }

    protected override void ReadyPlayerClient()
    {
        GameObject instance = PhotonNetwork.Instantiate("Character2 IcySliding", new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f)), Quaternion.identity);
        localPlayerCharacter = instance.GetComponent<PlayerCharacterControl2>();

        localPlayerCharacter.enabled = false; // 게임 시작 전까지 플레이어 컨트롤 비활성화
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
}
