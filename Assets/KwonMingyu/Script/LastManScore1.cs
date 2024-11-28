using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastManScore1 : MonoBehaviour
{
    [SerializeField] LastManPlayerCard1[] playerCards;

    private void OnEnable()
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            playerCards[player.GetPlayerNumber()].gameObject.SetActive(true);
            playerCards[player.GetPlayerNumber()].PlayerCardSetName(player);
        }
    }

    public void UpdateScore(Player player, int checkPointNum)
    {
        playerCards[player.GetPlayerNumber()].CheckPointIn(checkPointNum);
    }
    public void PlayerDead(Player player)
    {
        playerCards[player.GetPlayerNumber()].PlayerCardSetName(player, true);
    }

}
