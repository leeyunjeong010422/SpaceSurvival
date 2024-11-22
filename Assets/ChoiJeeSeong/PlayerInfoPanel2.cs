using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerInfoPanel2 : MonoBehaviour
{
    [SerializeField] PlayerInfoCell2 cellUIPrefab;

    private Dictionary<Player, PlayerInfoCell2> cells = null;

    public void InitRoomPlayerInfo()
    {
        if (cells != null)
        {
            Debug.Log("초기화 함수가 불필요하게 호출되었을 수 있음");
            foreach (var pair in cells)
            {
                Destroy(pair.Value.gameObject);
            }
            cells.Clear();
        }

        cells = new Dictionary<Player, PlayerInfoCell2>(PhotonNetwork.CountOfPlayersInRooms << 1);
        foreach (Player roomPlayer in PhotonNetwork.PlayerList)
        {
            // 플레이어마다 셀 생성 및 초기화
            PlayerInfoCell2 instance = Instantiate(cellUIPrefab, this.transform);
            instance.InitPlayerInfo(roomPlayer);
            instance.InfoText = "0";
            cells.Add(roomPlayer, instance);
        }
    }

    public void SetInt(Player player, int score)
    {
        if (cells.ContainsKey(player))
        {
            cells[player].InfoText = score.ToString();
        }
    }

    public void SetText(Player player, string text)
    {
        if (cells.ContainsKey(player))
        {
            cells[player].InfoText = text;
        }
    }
}
