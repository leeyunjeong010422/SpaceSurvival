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
    private void Start()
    {
        readyButton.onClick.AddListener(LocalPlayerReady);
    }

    public void CardInfoCanger(Player player)
    {
        playerName.text = player.NickName + (player.IsMasterClient ? "Host" : "");
        readyButton.interactable = player == PhotonNetwork.LocalPlayer;

        Cursor.lockState = CursorLockMode.None;
    }
    public void CardInfoReset()
    {
        playerName.text = "Empty";
        readyButton.interactable = false;
    }

    private void LocalPlayerReady()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        localPlayer.SetReady(!localPlayer.GetReady());
    }
}
