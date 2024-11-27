using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinningPointCell : MonoBehaviourPunCallbacks
{
    [Header("Child UI")]
    [SerializeField] TMP_Text nicknameText;
    [SerializeField] Image fillImage;
    [SerializeField] RawImage readyImage;

    private Player target;

    /// <summary>
    /// Ready 상태 추적 대상 결정 및 대상의 프로퍼티로부터 UI 설정
    /// </summary>
    /// <param name="target">대상</param>
    public void SetInfo(Player target)
    {
        this.target = target;
        nicknameText.text = target.NickName;
        readyImage.color = fillImage.color = target.GetNumberColor();
        fillImage.fillAmount = (float)target.GetWinningPoint() / (float)PhotonNetwork.CurrentRoom.GetGoalPoint();
        readyImage.gameObject.SetActive(target.GetReady());
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (target != targetPlayer)
            return;

        if (changedProps.ContainsKey(CustomProperty.READY))
        {
            readyImage.gameObject.SetActive(target.GetReady());
        }

        if (changedProps.ContainsKey(CustomProperty.WINNIN_POINT))
        {
            fillImage.fillAmount = (float)target.GetWinningPoint() / (float)PhotonNetwork.CurrentRoom.GetGoalPoint();
        }
    }
}
