using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersonalSettingPanel1 : MonoBehaviour
{
    [SerializeField] GameObject settingWindow;
    [SerializeField] TMP_InputField changeName;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingWindow.SetActive(!settingWindow.activeSelf);
        }
    }
    public void ChangeNickName()
    {
        if (string.IsNullOrEmpty(changeName.text)) return;
        BackendManager1.Instance.SetPlayerName(changeName.text);
    }
    public void LeftRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    private void OnDisable()
    {
        settingWindow.SetActive(false);
    }
}
