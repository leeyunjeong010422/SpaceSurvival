using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 미니게임 씬의 베이스 클래스<br/>
/// Start 구현시 override 해서 구현 및 base.Start 호출 요함
/// </summary>
public abstract class MiniGameSceneBase : MonoBehaviourPunCallbacks
{
    [SerializeField] AudioClip bgmClip;
    [SerializeField, Range(0f, 1f)] float bgmVolumeScale = 1f;

    private int clearCompleteCount = 0;

    protected virtual void Start()
    {
        // 이미 방에 들어가 있고
        // 게임에 참여할 플레이어가 모두 방에 들어가있어야 한다
        if (false == PhotonNetwork.InRoom)
        {
            Debug.LogWarning("방에 참여해 있지 않습니다");
            return;
        }

        // Ready를 기본값으로 변경
        // 게임 종료시 다음 게임으로 넘어가는데 사용
        PhotonNetwork.LocalPlayer.SetReady(false);

        if (PhotonNetwork.IsMasterClient)
        {
            ReadyNetworkScene();
        }

        ReadyPlayerClient();

        // 로딩 완료 통지
        PhotonNetwork.LocalPlayer.SetLoad(true);

        if (bgmClip != null)
        {
            GameManager.Sound.PlayBGM(bgmClip, bgmVolumeScale);
        }
    }

    protected virtual void OnDestroy()
    {
        GameManager.Sound.StopBGM();
    }

    /// <summary>
    /// 마스터 클라이언트에서 수행할 씬 초기화 작업<br/>
    /// 룸 오브젝트 생성 등
    /// </summary>
    protected abstract void ReadyNetworkScene();

    /// <summary>
    /// 각 클라이언트에서 수행할 씬 초기화 작업<br/>
    /// 플레이어 생성, RPC가 아닌 초기화 함수 호출 등
    /// </summary>
    protected abstract void ReadyPlayerClient();

    /// <summary>
    /// 각 클라이언트에서 게임 시작 시점에 할 작업(예: 게임 타이머 시작)
    /// </summary>
    protected abstract void GameStart();

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            if (CheckAllLoad())
                GameStart();
        }

        // 미니게임 종료 후 모두가 READY 상태라면 다음 미니게임으로
        if (PhotonNetwork.IsMasterClient && changedProps.ContainsKey(CustomProperty.READY))
        {
            if (CheckAllReady())
                LoadNextStage();
        }
    }

    private bool CheckAllLoad()
    {
        // PhotonNetwork.LevelLoadingProgress
        foreach (Player player in PhotonNetwork.PlayerList)
            if (!player.GetLoad()) return false;
        return true;
    }

    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
            if (!player.GetReady()) return false;
        return true;
    }

    /// <summary>
    /// 승점이 목표에 도달한 플레이어가 있다면 축하 씬으로, 없다면 다음 미니게임으로 진입<br/>
    /// 마스터 클라이언트가 아니라면 무시
    /// </summary>
    private void LoadNextStage()
    {
        if (false == PhotonNetwork.IsMasterClient)
            return;
        
        foreach (Player roomPlayer in PhotonNetwork.PlayerList)
        {
            roomPlayer.SetLoad(false);
            roomPlayer.SetReady(false);
        }

        photonView.RPC(nameof(ClearPhotonViewsRPC), RpcTarget.All);
    }

    [PunRPC]
    protected void ClearPhotonViewsRPC()
    {
        foreach (PhotonView view in PhotonNetwork.PhotonViewCollection)
        {
            if (view == photonView)
                continue;

            if (view.IsMine)
            {
                Debug.Log($"{view.name}제거");
                PhotonNetwork.Destroy(view);
            }
        }

        // 씬 정리가 완료되었음을 통지
        photonView.RPC(nameof(ClearCompleteRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    protected void ClearCompleteRPC(PhotonMessageInfo info)
    {
        clearCompleteCount++;

        // 모든 플레이어가 정리 완료시 씬 이동
        if (clearCompleteCount >= PhotonNetwork.PlayerList.Length)
        {
            // 승자 판정
            int goal = PhotonNetwork.CurrentRoom.GetGoalPoint();
            foreach (Player roomPlayer in PhotonNetwork.PlayerList)
            {
                if (goal <= roomPlayer.GetWinningPoint())
                {
                    // 로비씬에서 승리 이벤트 수행
                    PhotonNetwork.LoadLevel(0);
                    return;
                }
            }

            // 승자가 없으면 다음 미니게임으로
            PhotonNetwork.LoadLevel(1);
        }
    }
}
