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
    public void PopUpOpen(bool loadding, string info)
    {
        loaddingImg.SetActive(loadding);
        popUpInfo.text = info;
        popUp.SetActive(true);
        if (loadding)
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
