using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard1 : MonoBehaviour
{
    [SerializeField] Image playerImage;
    [SerializeField] TMP_Text playerName;
    [SerializeField] Button readyButton;
    [SerializeField] TMP_Text readyText;
    [SerializeField] Outline outline;

    private void Start()
    {
        // 플레이어 카드 버튼에 Ready 프로퍼티 변경 함수 추가
        readyButton.onClick.AddListener(LocalPlayerReady);
    }

    // 플레이어 카드 정보 변경 함수
    public void CardInfoCanger(Player player)
    {
        playerName.text = player.NickName + (player.IsMasterClient ? "Host" : "");
        readyButton.interactable = player == PhotonNetwork.LocalPlayer;
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
        readyButton.interactable = false;
        readyText.text = "준비안됨";
        readyText.color = Color.black;
        outline.effectColor = Color.black;
    }

    // 카드 버튼에 달릴 함수
    private void LocalPlayerReady()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        localPlayer.SetReady(!localPlayer.GetReady());
    }
}
