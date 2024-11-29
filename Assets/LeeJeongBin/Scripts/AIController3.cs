using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class AIController3 : MonoBehaviourPun
{
    public float moveSpeed;
    public float minPauseTime;
    public float maxPauseTime;
    public float rotationSpeed;

    [SerializeField] Vector3 targetPosition;
    [SerializeField] NavMeshAgent navMeshAgent;

    private Animator animator;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return; // 마스터 클라이언트에서만 AI 루프 실행

        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (navMeshAgent == null || animator == null)
            return;

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.autoBraking = false; // AI가 도착 지점에 도달하였을 경우 즉시 멈춤
        navMeshAgent.updateRotation = true;  // 회전을 NavMeshAgent에서 처리
        StartCoroutine(AILoop());
    }

    public void DieAICharacter()
    {
        photonView.RPC(nameof(DieAICharacterRPC), RpcTarget.All);
    }

    [PunRPC]
    private void DieAICharacterRPC()
    {
        // 사운드 재생(공통)

        if (false == PhotonNetwork.IsMasterClient)
            return;

        // 사망 처리(마스터)
        animator.SetTrigger("Die4");
        navMeshAgent.enabled = false;
        StartCoroutine(NetworkDestroyRoutine(5f));
    }

    private IEnumerator NetworkDestroyRoutine(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        PhotonNetwork.Destroy(this.gameObject);
    }

    private IEnumerator AILoop()
    {
        while (true)
        {
            // AI의 역동적 움직임을 위한 이동 후 멈춤 시간 설정
            float pauseTime = Random.Range(minPauseTime, maxPauseTime);
            UpdateAnimator(0f); // 멈춰있는 상태로 애니메이션 업데이트
            yield return new WaitForSeconds(pauseTime);

            // 목표 지점 설정 및 이동
            SetRandomTargetPosition();

            // 목표 지점을 설정할 수 있는지 확인
            if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
            {
                navMeshAgent.SetDestination(targetPosition);
            }

            // 네비 경로 완료 시까지 기다림
            while (navMeshAgent != null && navMeshAgent.isActiveAndEnabled && (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance))
            {
                if (navMeshAgent.isActiveAndEnabled)
                {
                    RotateTowardsTarget();
                    UpdateAnimator(navMeshAgent.velocity.magnitude);
                }
                yield return null;
            }

            // 목표에 도달한 후 0f ~ 3f 간격 만큼 랜덤 정지
            UpdateAnimator(0f);
            yield return new WaitForSeconds(Random.Range(0f, 3f));
        }
    }

    private void SetRandomTargetPosition()
    {
        float randomX = Random.Range(-45f, 45f);
        float randomZ = Random.Range(-45f, 45f);
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }

    private void RotateTowardsTarget()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimator(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed); // Speed 파라미터를 업데이트
        }
    }

    private void OnDisable()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled) // 비활성화 되면 경로를 업데이트 하지 않음
        {
            navMeshAgent.ResetPath(); // 경로를 초기화하여 잘못된 경로로 계속 이동하는 상황을 방지
        }
    }
}
