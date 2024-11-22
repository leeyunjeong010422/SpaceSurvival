using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MainPanel1 : MonoBehaviour
{
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text maxPlayerText;
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] TMP_Text levelText;
    private int maxPlayer;
    
    private void OnEnable()
    {
        PlayerInfoSetting();
    }
    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);
        roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
        maxPlayerText.text = "4";
        maxPlayer = 4;
    }
    public void MaxPlayerCanger(bool up)
    {
        if (up) maxPlayer++;
        else maxPlayer--;
        maxPlayer = Mathf.Clamp(maxPlayer, 2, 4);
        maxPlayerText.text = maxPlayer.ToString();
    }

    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        if (roomName == "") return;

        RoomOptions roomOptions = new();
        roomOptions.MaxPlayers = maxPlayer;

        createRoomPanel.SetActive(false);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    public void RandomMatching()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }
    private async void PlayerInfoSetting()
    {
        UserData1 userData = (await BackendManager1.Instance.GetPlayerData());
        nickNameText.text = userData.name;
        levelText.text = "LV " + userData.level;
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
