using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoCell2 : MonoBehaviour
{
    public string NicknameText { set => nicknameText.text = value; }
    public string InfoText { set => scoreText.text = value; }
    public Color PersonalColor { set => colorImage.color = value; }

    [SerializeField] TMP_Text nicknameText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Image colorImage;

    public void InitPlayerInfo(Player player)
    {
        NicknameText = player.NickName;
        PersonalColor = player.GetNumberColor();
    }
}
