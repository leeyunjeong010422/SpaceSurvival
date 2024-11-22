using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUp1 : MonoBehaviour
{
    public static PopUp1 Instance;
    [SerializeField] GameObject popUp;
    [SerializeField] GameObject loaddingImg;
    [SerializeField] TMP_Text popUpInfo;
    private void Awake()
    {
        Instance = this;
    }
    public void PopUpOpen(bool loadding, string info)
    {
        loaddingImg.SetActive(loadding);
        popUpInfo.text = info;
        popUp.SetActive(true);
    }
    public void PopUpClose()
    {
        popUp.SetActive(false);
    }
}
