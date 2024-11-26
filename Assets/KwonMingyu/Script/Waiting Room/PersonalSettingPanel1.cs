using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalSettingPanel1 : MonoBehaviour
{
    // Esc로 열수있는 창 전체 오브젝트
    [SerializeField] GameObject settingWindow;

    // 캐릭터 색갈을 결정하기 전 미리 볼 수 있는 오브젝트
    [SerializeField] GameObject characterModel;

    // 선택된 색갈을 알려주는 아웃라인
    [SerializeField] Outline[] colorBlocks;

    // 선택된 색갈의 아웃라인 색갈
    [SerializeField] Color selectBlockColor;

    // 색갈 변경 버튼
    [SerializeField] Button ColorChangeButton;

    // 다른 플레이어와 중복된 색갈인지 알려주는 텍스트
    [SerializeField] GameObject overlapText;

    // 선택 가능한 색갈들
    [SerializeField] Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue, Color.cyan, Color.black, Color.white, Color.gray };

    // 선택된 색갈의 인덱스
    [SerializeField] int selectColorNum = 0;

    // GoolScore룸 프로퍼티 설정을 위한 변수
    [SerializeField] TMP_Text goalScoreText;
    [SerializeField] int goalScore = 30;

    // 호스트만 출력할 오브젝트
    [SerializeField] GameObject[] roomSettingButtons;

    private void OnEnable()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;
        foreach (var item in roomSettingButtons)
            item.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingWindow.SetActive(!settingWindow.activeSelf);
            Cursor.lockState = settingWindow.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
            goalScoreText.text = $"{PhotonNetwork.CurrentRoom.GetGoalPoint()}";
        }
    }
    public void ColorChange(bool next)
    {
        // 버튼 설정에 따라서 +- 결정
        selectColorNum += next ? 1 : -1;

        // 인덱스 이탈 방지
        if (selectColorNum >= colors.Length) selectColorNum = 0;
        if (selectColorNum < 0) selectColorNum = colors.Length - 1;

        // 아웃라인 활성화
        OutLineSet(selectColorNum);
        // 모델 색상 변경
        characterModel.GetComponent<Renderer>().material.color = colors[selectColorNum];
        // 다른 플레이어와 컬러 중복 체크
        ColorOverlapCheck(selectColorNum);
    }
    private void OutLineSet(int select)
    {
        foreach (var item in colorBlocks)
        {
            item.effectColor = Color.black;
        }
        colorBlocks[select].effectColor = selectBlockColor;
    }
    private void ColorOverlapCheck(int selectColor)
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (selectColor == player.GetColorNumber())
            {
                overlapText.SetActive(true);
                ColorChangeButton.interactable = false;
                return;
            }
        }
        overlapText.SetActive(false);
        ColorChangeButton.interactable = true;
    }
    public void PlayerColorChange()
    {
        PhotonNetwork.LocalPlayer.SetColorNumber(selectColorNum);

        // Set 후 곧바로 Get 호출시 이전 값으로 반환됨 ColorOverlapCheck하지 않고 바로 버튼을 비활성화
        overlapText.SetActive(true);
        ColorChangeButton.interactable = false;
    }
    public void RoomGoalScoreChange(bool up)
    {
        goalScore += 5 * (up ? 1 : -1);
        goalScore = Mathf.Clamp(goalScore, 5, 100);
        goalScoreText.text = $"{goalScore}";
    }
    public void RoomGoalScoreSet()
    {
        PhotonNetwork.CurrentRoom.SetGoalPoint(goalScore);
    }
    public void LeftRoom()
    {
        // 변경된 프로퍼티를 초기화
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetColorNumber(-1);
        
        // 룸 이탈 후 프로퍼티 조작시 오류 발생, 초기화 이후에 룸 나가기
        PhotonNetwork.LeaveRoom();
    }

    // 방을 나갈때 열린 설정창이 닫히도록
    private void OnDisable()
    {
        settingWindow.SetActive(false);
    }
}
