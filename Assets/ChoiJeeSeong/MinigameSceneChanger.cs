using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameSceneChanger : MonoBehaviour
{
    private IEnumerator Start()
    {
        MinigameSelecter.Minigame nextGame = MinigameSelecter.Instance.PopRandom();
        Debug.Log($"다음 스테이지: {nextGame}");
        MinigameSelecter.MinigameData nextGameData = MinigameSelecter.Instance.GetData(nextGame);

        MiniGameInfoUI1 infoUI = Instantiate(nextGameData.infoPrefab);
        DontDestroyOnLoad(infoUI);
        SceneManager.sceneLoaded += (scene, mode) => Destroy(infoUI, 5f);
        infoUI.FadeIn();

        yield return new WaitForSeconds(2f);

        infoUI.FadeOut();

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(nextGameData.buildIndex);
        }
    }
}
