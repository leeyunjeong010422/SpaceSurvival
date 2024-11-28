using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard1 : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text readyText;
    [SerializeField] Outline outline;

    // 플레이어 카드 정보 변경 함수
    public void CardInfoCanger(Player player)
    {
        playerName.text = player.NickName + (player.IsMasterClient ? "Host" : "");
        if (player.GetReady())
        {
            readyText.text = "준비됨";
            readyText.color = Color.red;
        }
        else
        {
            readyText.text = "준비안됨";
            readyText.color = Color.black;
        }
    }
    public void CardOutLineSet(Color color)
    {
        outline.effectColor = color;
    }

    // 플레이어 카드 초기화 함수
    public void CardInfoReset()
    {
        playerName.text = "Empty";
        readyText.text = "";
        readyText.color = Color.black;
        outline.effectColor = Color.black;
    }
}
