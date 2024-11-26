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
        CheckAndRemoveTargetInRange();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void CheckAndRemoveTargetInRange() // 타겟 확인 후 범위 벗어난 타겟을 리스트에서 제거
    {
        SphereCollider sphereCollider = attackRange.GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            Collider[] colliders = Physics.OverlapSphere(attackRange.transform.position, sphereCollider.radius);

            // 새로운 타겟을 찾고 리스트에 추가
            foreach (Collider collider in colliders)
            {
                GameObject target = collider.gameObject;

                // 본인을 제외한 범위 내에 들어온 타겟을 제거
                if (target != gameObject && target != null && !targetInRange.Contains(target))
                {
                    targetInRange.Add(target);
                }
            }

            // 범위에서 벗어난 타겟들을 제거
            List<GameObject> toRemoveList = new List<GameObject>();

            foreach (GameObject target in targetInRange)
            {
                if (target == null || !IsTargetInRange(target))  // 타겟이 null이거나 범위에서 벗어난 경우
                {
                    toRemoveList.Add(target); // 리스트에 추가
                }
            }

            // 범위에서 벗어난 타겟들을 리스트에서 제거
            foreach (GameObject target in toRemoveList)
            {
                if (target != null)  // null인 타겟을 처리하지 않음
                {
                    targetInRange.Remove(target);
                }
            }
        }
    }

    // 공격 범위(Spere 콜라이더) 내에 있는 타겟이 여전히 범위 내에 있는지
    private bool IsTargetInRange(GameObject target)
    {
        if (target == null) return false;  // 타겟이 nuul이면 범위 내에 없는 것으로 간주하고 반환
        SphereCollider sphereCollider = attackRange.GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            return Vector3.Distance(target.transform.position, attackRange.transform.position) <= sphereCollider.radius;
        }
        return false;
    }

    // 공격 처리
    private void Attack()
    {
        List<GameObject> targetsToAttack = new List<GameObject>(targetInRange);

        foreach (GameObject target in targetsToAttack)
        {
            // 타겟이 null이 아닌지 확인
            if (target != null && (target.CompareTag("AI") || target.CompareTag("Player")))
            {
                HandleTargetDeath(target);
            }
        }
    }

    private void HandleTargetDeath(GameObject target)
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
