using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocalPlayerTrigger2 : MonoBehaviour
{
    // 씬 관리자에서 RPC를 통해 관리하기 위해 이벤트로 노출해서 외부에서 습득 처리
    // 거의 동시에 한 코인을 주울 경우 A 클라이언트에서 이미 사라진 코인이 B 클라이언트에서 점수를 추가하려 하는 경우의 예외 처리가 필요하다
    public UnityEvent<int> OnLocalPlayerTriggered;

    public int Id { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        // 로컬 플레이어 캐릭터의 레이어를 분리하는 것이 더 유리할수도?
        Rigidbody rigidbody = other.attachedRigidbody;
        if (rigidbody != null && rigidbody.CompareTag("Player") // 대상이 플레이어인지 검사
            && rigidbody.TryGetComponent(out PhotonView photonView) && photonView.IsMine) // 대상이 LocalPlayer인지 검사
        {
            OnLocalPlayerTriggered?.Invoke(Id);
        }
    }
}
