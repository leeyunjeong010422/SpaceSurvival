using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSampleScene : MiniGameSceneBase
{
    [SerializeField] ScorePanel scorePanel;
    [SerializeField] ScoreSampleButtons sampleButtonPanel;
    
    private MiniGameScore scoreManager;

    private void Awake()
    {
        scoreManager = GetComponent<MiniGameScore>();
    }

    protected override void Start()
    {
        base.Start();

    }

    protected override void ReadyNetworkScene()
    {
    }

    protected override void ReadyPlayerClient()
    {
        scoreManager.InitScoreTable();

        foreach (Photon.Realtime.Player roomPlayer in PhotonNetwork.PlayerList)
        {
            scorePanel.AddPlayer(roomPlayer);
        }

        scoreManager.OnScoreChanged.AddListener(scorePanel.UpdateScoreUI);

        sampleButtonPanel.WhenTestStarted();
    }

    protected override void GameStart()
    {
        Debug.Log("MiniGameScore 클래스 사용 샘플 테스트 시작");
    }
}
