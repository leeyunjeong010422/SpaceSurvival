using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController3 : MonoBehaviour
{
    public float moveSpeed;
    public float minPauseTime;
    public float maxPauseTime;
    public float rotationSpeed;

    [SerializeField] Vector3 targetPosition;
    [SerializeField] NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
            return;

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.autoBraking = false; // AI가 도착 지점에 도달하였을 경우 즉시 멈춤
        navMeshAgent.updateRotation = true;  // 회전을 NavMeshAgent에서 처리
        StartCoroutine(AILoop());
    }

    private IEnumerator AILoop()
    {
        while (true)
        {
            // AI의 역동적 움직임을 위한 이동 후 멈춤 시간 설정
            float pauseTime = Random.Range(minPauseTime, maxPauseTime);
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
                }
                yield return null;
            }

            // 목표에 도달한 후 1f ~ 3f 만큼 랜덤 정지
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private void SetRandomTargetPosition()
    {
        float randomX = Random.Range(-50f, 50f);
        float randomZ = Random.Range(-50f, 50f);
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

    private void OnDisable()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled) // 비활성화 되면 경로를 업데이트 하지 않음
        {
            navMeshAgent.ResetPath(); // 경로를 초기화하여 잘못된 경로로 계속 이동하는 상황을 방지
        }
    }
}
