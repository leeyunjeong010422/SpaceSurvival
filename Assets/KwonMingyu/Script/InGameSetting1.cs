using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSetting1 : MonoBehaviour
{
    [SerializeField] GameObject settingPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingPanel.SetActive(!settingPanel.activeSelf);
        }
    }
    public void LeftRoom()
    {
        // 변경된 프로퍼티를 초기화
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetColorNumber(-1);

        // 룸 이탈 후 프로퍼티 조작시 오류 발생, 초기화 이후에 룸 나가기
        PhotonNetwork.LeaveRoom();
    }
}
