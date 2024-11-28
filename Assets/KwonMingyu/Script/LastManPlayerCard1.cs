using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LastManPlayerCard1 : MonoBehaviour
{
    [SerializeField] Image[] pointImg;
    [SerializeField] TMP_Text playerName;
    [SerializeField] Color[] checkPointColors;
    public void PlayerCardSetName(Player player, bool dead = false)
    {
        playerName.color = player.GetNumberColor();
        playerName.text = player.NickName + (dead ? " 사망" : "");
    }
    public void CheckPointIn(int checkPointNum)
    {
        pointImg[checkPointNum].color = checkPointColors[checkPointNum];
    }

}
