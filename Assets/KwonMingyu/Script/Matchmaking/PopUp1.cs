using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp1 : MonoBehaviour
{
    public static PopUp1 Instance;
    [SerializeField] GameObject popUp;
    [SerializeField] GameObject loaddingImg;
    [SerializeField] TMP_Text popUpInfo;
    [SerializeField] Button closeButton;
    [SerializeField] Button backgroundButton;
    private void Awake()
    {
        Instance = this;
    }
    public void PopUpOpen(bool loadding, string info, bool notClose = false)
    {
        loaddingImg.SetActive(loadding);
        popUpInfo.text = info;
        popUp.SetActive(true);
        if (notClose)
        {
            closeButton.gameObject.SetActive(false);
            backgroundButton.onClick.RemoveAllListeners();
        }
        else
        {
            closeButton.gameObject.SetActive(true);
            backgroundButton.onClick.AddListener(PopUpClose);
        }
    }
    public void PopUpClose()
    {
        popUp.SetActive(false);
    }
}
