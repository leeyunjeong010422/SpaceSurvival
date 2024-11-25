using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController3 : MonoBehaviourPun
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float deceleration;
    [SerializeField] Camera playerCamera;
    [SerializeField] CharacterController characterController;
    [SerializeField] Vector3 velocity;

    private PhotonTransformView photonTransformView;

    [SerializeField] int checkPointsReached;

    private static List<CheckPoint3> visitedCheckPoint = new List<CheckPoint3>();

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        photonTransformView = GetComponent<PhotonTransformView>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            velocity.x = moveDirection.x * moveSpeed;
            velocity.z = moveDirection.z * moveSpeed;
        }
        else
        {
            velocity = Vector3.zero;
        }

        if (characterController.isGrounded)
        {
            velocity.y = -0.5f;
        }
        else
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    public void OnTriggerCheckPoint(CheckPoint3 checkPoint)
    {
        if (visitedCheckPoint.Contains(checkPoint))
        {
            return;
        }

        checkPointsReached++;
        visitedCheckPoint.Add(checkPoint);

        Debug.Log($"플레이어 {photonView.Owner.NickName} 체크포인트 {checkPointsReached}/{checkPoint.TotalCheckPoints} 통과");

        if (checkPointsReached >= checkPoint.TotalCheckPoints)
        {
            Debug.Log($"플레이어 {photonView.Owner.NickName}가 모든 체크포인트를 통과했습니다");
            GameOver3.Instance.PlayerWin(photonView.Owner.NickName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            CheckPoint3 checkPoint = other.GetComponent<CheckPoint3>();
            if (checkPoint != null)
            {
                OnTriggerCheckPoint(checkPoint);
            }
        }
    }

    public static void ResetCheckPoints()
    {
        visitedCheckPoint.Clear();
    }
}
