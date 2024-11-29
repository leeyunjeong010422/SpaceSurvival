using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Killing3 : MonoBehaviourPun
{
    [Header("공격 범위 설정")]
    [SerializeField] private GameObject attackRange;
    private HashSet<GameObject> targetInRange = new HashSet<GameObject>();

    private Animator animator;
    private bool IsAttack = false;

    private Coroutine playerDeadCoroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return; // 내 플레이어가 아닌 경우 공격 무시

        if (Input.GetMouseButtonDown(0) && !IsAttack) 
        {
            StartCoroutine(AttackDelay());

            attackRange.SetActive(true);

            if (animator != null)
            {
                // 어택 애니메이션 트리거
                animator.SetTrigger("MeleeAttack");
                photonView.RPC("SyncTrigger", RpcTarget.All, "MeleeAttack");

                // 레이어2 활성화
                animator.SetLayerWeight(2, 1f);
                photonView.RPC("SetLayerWeight", RpcTarget.All, 2, 1f);
            }
        }


        if (Input.GetMouseButtonUp(0))
        {
            attackRange.SetActive(false);

            // 애니메이션 종료 후 레이어2를 비활성화
            if (animator != null)
            {
                animator.SetTrigger("Idle4");
                photonView.RPC("SyncTrigger", RpcTarget.All, "Idle4");

                // 애니메이션 종료 후 레이어2 비활성화
                StartCoroutine(WaitForAnimation());
            }
        }
    }

    private IEnumerator AttackDelay()
    {
        IsAttack = true;

        yield return new WaitForSeconds(1f);

        IsAttack = false;
    }

    // 애니메이션이 끝날 때까지 기다린 후 레이어 비활성화
    private IEnumerator WaitForAnimation()
    {
        // 어태기 애니메이션이 끝날 때까지 기다리기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(2); // 레이어2 에서 진행 중인 애니메이션
        float animationLength = stateInfo.length;

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animationLength);

        // 애니메이션이 끝난 후 레이어2 비활성화
        animator.SetLayerWeight(2, 0f);
        photonView.RPC("SetLayerWeight", RpcTarget.All, 2, 0f);
    }

    [PunRPC]
    private void SetLayerWeight(int layerIndex, float weight)
    {
        if (animator != null)
        {
            animator.SetLayerWeight(layerIndex, weight);
        }
    }

    [PunRPC]
    private void SyncTrigger(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
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
                target.GetComponent<CapsuleCollider>().enabled = false;
                if (target.CompareTag("AI"))
                {
                    StartCoroutine(HandleAIDeath(target.GetComponent<AIController3>()));
                }
                else if (target.CompareTag("Player"))
                {
                    target.GetComponent<PlayerController3>().score.PlayerDead(targetPhotonView.Owner);
                    target.GetComponent<PlayerController3>().DeadPlayer();
                }
            }
        }
    }

    // AI 사망시 5초뒤 Destroy 및 사망 애니메이션
    private IEnumerator HandleAIDeath(AIController3 aiController)
    {
        Animator animator = aiController.GetComponent<Animator>();
        animator.SetTrigger("Die4");

        aiController.GetComponent<NavMeshAgent>().enabled = false;

        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(aiController.gameObject);
        }
    }
}
