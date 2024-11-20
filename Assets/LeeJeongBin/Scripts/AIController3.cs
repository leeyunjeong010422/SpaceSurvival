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
    private bool isMoving = false;

    void Start()
    {
        StartCoroutine(AIBehaviorLoop());
    }

    private IEnumerator AIBehaviorLoop()
    {
        while (true)
        {
            isMoving = false;
            float pauseTime = Random.Range(minPauseTime, maxPauseTime);
            yield return new WaitForSeconds(pauseTime);

            isMoving = true;
            SetRandomTargetPosition();
            float moveDuration = Random.Range(2f, 5f);
            yield return MoveToTarget(moveDuration);
        }
    }

    private void SetRandomTargetPosition()
    {
        float randomX = Random.Range(-10f, 10f);
        float randomZ = Random.Range(-10f, 10f);
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }

    private IEnumerator MoveToTarget(float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            if (!isMoving) break;

            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            yield return null;
        }
    }
}
