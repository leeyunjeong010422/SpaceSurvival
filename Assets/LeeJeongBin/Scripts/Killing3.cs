using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Killing3 : MonoBehaviourPun
{
    [Header("공격 범위 설정")]
    [SerializeField] private GameObject attackRange;
    private HashSet<GameObject> targetInRange = new HashSet<GameObject>();

    private void Update()
    {
        if (!photonView.IsMine)
            return; // 내 플레이어가 아닌 경우 공격 무시

        if (Input.GetMouseButtonDown(0))
        {
            attackRange.SetActive(true);
            Debug.Log("공격");
        }
        if (Input.GetMouseButtonUp(0))
        {
            attackRange.SetActive(false);
        }
    }

    public void HandleTargetDeath(GameObject target)
    {
        if (target == null)
        {
            return;  // 타겟이 null이면 더 이상 처리하지 않고 반환
        }

        PhotonView targetPhotonView = target.GetComponent<PhotonView>();

        if (targetPhotonView != null)
        {
            // AI는 소유권과 상관없이 죽일 수 있음(AI 소유권은 마스터 클라이언트에게 있음)
            if (target.CompareTag("AI") || (targetPhotonView.IsMine == false && target.CompareTag("Player")))
            {
                // 타겟이 AI일 경우 제거 및 NavMeshAgent 비활성화
                AIController3 aiController = target.GetComponent<AIController3>();
                if (aiController != null)
                {
                    aiController.GetComponent<NavMeshAgent>().enabled = false;
                }

                photonView.RPC("DestroyTarget", RpcTarget.AllBuffered, targetPhotonView.ViewID);
            }
        }
    }

    [PunRPC]
    private void DestroyTarget(int targetViewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView != null)
        {
            GameObject target = targetPhotonView.gameObject;

            // 타겟이 제거된 경우 참조하지 않음
            if (target != null)
            {
                Destroy(target);
            }
        }
    }
}
