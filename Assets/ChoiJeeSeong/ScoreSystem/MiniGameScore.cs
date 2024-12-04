using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameScore : MonoBehaviourPun
{
    public UnityEvent<Player, int> OnScoreChanged;

    /// <summary>
    /// 플레이어의 ActorNumber를 키 값으로 점수를 저장
    /// </summary>
    public Dictionary<int, int> ScoreTable { get; private set; }

    /// <summary>
    /// 점수 테이블 초기화. PhotonNetwork에서 Room 참여 상태여야 한다
    /// </summary>
    public void InitScoreTable()
    {
        if (false == PhotonNetwork.InRoom)
        {
            Debug.LogWarning($"방에 참여해 있지 않은 상태");
            return;
        }

        ScoreTable = new Dictionary<int, int>(PhotonNetwork.CountOfPlayersInRooms << 1);
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            ScoreTable.Add(player.ActorNumber, 0);
        }
    }

    /// <summary>
    /// localPlayer에게 점수 추가
    /// </summary>
    /// <param name="value">가산할 점수</param>
    public void AddScore(int value)
    {
        // 자신의 점수 획득 사실을 공유한다
        photonView.RPC(nameof(AddScoreRPC), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, value);
    }

    [PunRPC]
    private void AddScoreRPC(int actorNumber, int value)
    {
        ScoreTable[actorNumber] += value;
        OnScoreChanged?.Invoke(PhotonNetwork.CurrentRoom.GetPlayer(actorNumber), ScoreTable[actorNumber]);
    }

    [ContextMenu("(테스트)Add Score 3")]
    private void AddScoreTest()
    {
        AddScore(3);
    }
}
