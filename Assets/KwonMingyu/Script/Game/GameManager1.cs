using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoad(true);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty1.LOAD))
        {
            if (CheckAllLoad())
                GameStart();
        }
    }
    private bool CheckAllLoad()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
            if (!player.GetLoad()) return false;
        return true;
    }
    private void GameStart()
    {
        PhotonNetwork.Instantiate("Cube", Vector3.zero, Quaternion.identity);
    }
}
