using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinningPointPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] WinningPointCell cellPrefab;

    [Header("Child UI")]
    [SerializeField] LayoutGroup layoutGroup;

    public void OnPointerClick(PointerEventData eventData)
    {
        // IPointerClickHandler
        eventData.Use();
        PhotonNetwork.LocalPlayer.SetReady(true);
    }

    private void Start()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            WinningPointCell cellInstance = Instantiate(cellPrefab, layoutGroup.transform);
            cellInstance.SetInfo(player);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
        }
    }
}
