using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel1 : MonoBehaviour
{
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text maxPlayerText;
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_InputField nickChangeField;
    [SerializeField] Toggle visibleToggle;
    [SerializeField] TMP_InputField joinToRoomNameInputField;
    private int maxPlayer;
    
    // 메인 화면으로 이동할 때 우상단의 플레이어 정보를 입력
    private void OnEnable()
    {
        PlayerInfoSetting();
    }
    // 메인 화면에서 방 생성 버튼을 눌렀을 때
    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);
        roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
        maxPlayerText.text = "4";
        maxPlayer = 4;
    }
    // 버튼으로 최대 인원수를 변경하기 위한 함수
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
        roomOptions.IsVisible = !visibleToggle.isOn;

        createRoomPanel.SetActive(false);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        PopUp1.Instance.PopUpOpen(true, "방을 생성하고 있어요");
    }
    public void RandomMatching()
    {
        PhotonNetwork.JoinRandomRoom();
        PopUp1.Instance.PopUpOpen(true, "랜덤 방에 접속하고 있어요");
    }
    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }
    public void JoinRoomToName()
    {
        Debug.Log(PhotonNetwork.JoinRoom(joinToRoomNameInputField.text));
        joinToRoomNameInputField.transform.parent.gameObject.SetActive(false);
    }
    // 플레이어 정보 세팅을 위한 함수
    private async void PlayerInfoSetting()
    {
        UserData1 userData = (await BackendManager1.Instance.GetPlayerData());
        PhotonNetwork.LocalPlayer.NickName = userData.name;
        nickNameText.text = userData.name;
        levelText.text = "LV " + userData.level;
    }
    // 닉네임 변경을 위한 함수
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
