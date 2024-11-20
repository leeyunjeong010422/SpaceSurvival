using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController3 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float minPauseTime;
    public float maxPauseTime;

    private Vector3 targetPosition;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        StartCoroutine(AILoop());
    }

    private IEnumerator AILoop()
    {
        while (true)
        {
            navMeshAgent.isStopped = true;
            float pauseTime = Random.Range(minPauseTime, maxPauseTime);
            yield return new WaitForSeconds(pauseTime);

            SetRandomTargetPosition();
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(targetPosition);

            float moveDuration = Random.Range(2f, 5f);
            yield return new WaitForSeconds(moveDuration);
        }
    }

    private void SetRandomTargetPosition()
    {
        float randomX = Random.Range(-50f, 50f);
        float randomZ = Random.Range(-50f, 50f);
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }
}
