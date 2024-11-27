using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameSceneChanger : MonoBehaviourPun
{
    private MiniGameInfoUI1 infoUI;

    private IEnumerator Start()
    {
        // RPC 호출은 마스터에서만
        if (false == PhotonNetwork.IsMasterClient)
            yield break;


        MinigameSelecter.Minigame nextGame = MinigameSelecter.Instance.PopRandom();
        Debug.Log($"다음 스테이지: {nextGame}");
        photonView.RPC(nameof(SetInfoUIRPC), RpcTarget.All, nextGame);
    }

    [PunRPC]
    private void SetInfoUIRPC(MinigameSelecter.Minigame nextGame) => StartCoroutine(SetInfoUIRoutine(nextGame));
    
    private IEnumerator SetInfoUIRoutine(MinigameSelecter.Minigame nextGame)
    {
        MinigameSelecter.MinigameData nextGameData = MinigameSelecter.Instance.GetData(nextGame);

        infoUI = Instantiate(nextGameData.infoPrefab);
        DontDestroyOnLoad(infoUI);

        // 씬이 변경된 시점에서 FadeOut 호출
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        infoUI.FadeIn();

        yield return new WaitForSeconds(2f);

        // FadeOut을 여기서 호출했을 때, 마스터 이외의 클라이언트에서 코루틴의 해당 구간 진입 이전에 씬이 전환되어 FadeOut이 호출되지 못하는 경우가 발생

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(nextGameData.buildIndex);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

        infoUI.FadeOut();
        Destroy(infoUI.gameObject, 5f);
    }
}
