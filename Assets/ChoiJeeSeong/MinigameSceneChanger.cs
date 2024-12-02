using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameSceneChanger : MonoBehaviourPun
{
    private MiniGameInfoUI1 infoUI;
    private int clearCompleteCount;

    private IEnumerator Start()
    {
        // 미니게임에서 사용되는 프로퍼티 디폴트 값으로 변경
        PhotonNetwork.LocalPlayer.SetLoad(false);
        PhotonNetwork.LocalPlayer.SetReady(false);

        // RPC 호출은 마스터에서만
        if (false == PhotonNetwork.IsMasterClient)
            yield break;

        MinigameSelecter.Minigame nextGame = MinigameSelecter.Instance.PopRandom();
        Debug.Log($"다음 스테이지: {nextGame}");
        photonView.RPC(nameof(ReadyNextScene), RpcTarget.All, nextGame);
    }

    [PunRPC]
    private void ReadyNextScene(MinigameSelecter.Minigame nextGame) => StartCoroutine(ReadyNextSceneRoutine(nextGame));
    
    private IEnumerator ReadyNextSceneRoutine(MinigameSelecter.Minigame nextGame)
    {
        MinigameSelecter.MinigameData nextGameData = MinigameSelecter.Instance.GetData(nextGame);

        infoUI = Instantiate(nextGameData.infoPrefab);
        DontDestroyOnLoad(infoUI);

        // 씬이 변경된 시점에서 FadeOut 호출
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        infoUI.FadeIn();

        // 룸 자체에 남아있는 PhotonView 제거
        ClearPhotonView();

        yield return new WaitForSeconds(2f);

        // FadeOut을 여기서 호출했을 때, 마스터 이외의 클라이언트에서 코루틴의 해당 구간 진입 이전에 씬이 전환되어 FadeOut이 호출되지 못하는 경우가 발생

        if (PhotonNetwork.IsMasterClient)
        {
            // 모두가 정리 완료되었는지 검사
            YieldInstruction wait = new WaitForSeconds(0.1f);
            while (clearCompleteCount < PhotonNetwork.CountOfRooms)
                yield return wait;

            PhotonNetwork.LoadLevel(nextGameData.buildIndex);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

        infoUI.FadeOut();
        Destroy(infoUI.gameObject, 5f);
    }

    private void ClearPhotonView()
    {
        foreach (PhotonView view in PhotonNetwork.PhotonViewCollection)
        {
            if (view == photonView)
                continue;

            if (view.IsMine)
                PhotonNetwork.Destroy(view);
        }

        // 씬 정리가 완료되었음을 통지
        photonView.RPC(nameof(ClearCompleteRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void ClearCompleteRPC(PhotonMessageInfo info)
    {
        clearCompleteCount++;
    }
}
