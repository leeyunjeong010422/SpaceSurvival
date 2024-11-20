using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerState1 : MonoBehaviourPunCallbacks
{
    [SerializeField] ClientState state;

    void Update()
    {
        if (state == PhotonNetwork.NetworkClientState) return;

        state = PhotonNetwork.NetworkClientState;
        Debug.Log(state);
    }
}
