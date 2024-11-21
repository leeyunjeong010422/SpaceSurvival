using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorTest : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.IsConnected);

        // rgb에 랜덤값 넣기
        PhotonNetwork.LocalPlayer.SetColorR(Random.Range(0f, 1f));
        PhotonNetwork.LocalPlayer.SetColorG(Random.Range(0f, 1f));
        PhotonNetwork.LocalPlayer.SetColorB(Random.Range(0f, 1f));
    }
}
