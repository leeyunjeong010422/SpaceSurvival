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
    [SerializeField] TMP_InputField nickChangeField;
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
        PopUp1.Instance.PopUpOpen(true, "방을 생성하고 있어요");
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
    public async void ChangeNickName()
    {
        if (await BackendManager1.Instance.SetPlayerName(nickChangeField.text))
        {
            PlayerInfoSetting();
            PopUp1.Instance.PopUpOpen(false, "닉네임 변경에 성공했어요");
            nickChangeField.transform.parent.gameObject.SetActive(false);
            return;
        }
        PopUp1.Instance.PopUpOpen(false, "닉네임 변경에 실패했어요");
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
