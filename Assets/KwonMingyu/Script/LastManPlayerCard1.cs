using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LastManPlayerCard1 : MonoBehaviour
{
    [SerializeField] Image[] pointImg;
    private TMP_Text playerName;
    [SerializeField] Color[] checkPointColors;

    private void Awake()
    {
        playerName = transform.GetComponentInChildren<TMP_Text>();
    }
    public void CheckPointIn(int pointNum)
    {
        pointImg[pointNum].color = checkPointColors[pointNum];
    }

}
