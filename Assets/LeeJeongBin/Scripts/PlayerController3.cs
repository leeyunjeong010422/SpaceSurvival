using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController3 : MonoBehaviourPun
{
    [SerializeField] float moveSpeed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float rotationSpeed;
    [SerializeField] Camera playerCamera;
    [SerializeField] Vector3 velocity;

    private CharacterController characterController;
    private PhotonTransformView photonTransformView;
    private Animator animator;

    [SerializeField] int checkPointsReached;

    public List<CheckPoint3> visitedCheckPoint = new List<CheckPoint3>();

    public LastManScore1 score;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        photonTransformView = GetComponent<PhotonTransformView>();
        animator = GetComponent<Animator>();
        GameOver3.Instance.OnPlayerSpawn(this);

        score = FindObjectOfType<LastManScore1>();
    }

    private void OnDestroy()
    {
        GameOver3.Instance?.OnPlayerDeath(this);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
            UpdateAnimator();
        }
        else
        {
            velocity = Vector3.zero; // 네트워크 상 다른 플레이어는 로컬에서 움직이지 않음
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

        float currentSpeed = moveSpeed;

        // Shift 키를 누르면 스프린트 활성화
        if (Input.GetKey(KeyCode.LeftShift) && vertical > 0) // W 키와 함께 Shift를 누른 경우에만
        {
            currentSpeed *= sprintMultiplier; // 기본 속도에 스프린트 배율 적용 (최대 3.5)
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            velocity.x = moveDirection.x * currentSpeed;
            velocity.z = moveDirection.z * currentSpeed;
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        if (characterController.isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        // 이동 속도 계산
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        animator.SetFloat("Speed", speed);
    }

    public void OnTriggerCheckPoint(CheckPoint3 checkPoint)
    {
        if (visitedCheckPoint.Contains(checkPoint))
        {
            return;
        }

        // 체크포인트 진입시 전체 알림
        photonView.RPC(nameof(TriggerCheckPointRPC), RpcTarget.All, checkPoint.CheckPointNum);

        checkPointsReached++;
        visitedCheckPoint.Add(checkPoint);
        
        if (checkPointsReached >= checkPoint.TotalCheckPoints)
        {
            Debug.Log($"모든 체크포인트를 통과했습니다");
            GameOver3.Instance.PlayerWin(photonView.Owner);
        }
    }

    [PunRPC]
    private void TriggerCheckPointRPC(int checkPointNum)
    {
        // 사운드 재생 필요
        Debug.Log($"플레이어 {photonView.Owner.NickName} 체크포인트 통과");
        score.UpdateScore(photonView.Owner, checkPointNum);

    }

    public void ResetCheckPoints()
    {
        visitedCheckPoint.Clear();
    }

    public int CheckPointsReached
    {
        get { return checkPointsReached; }
        set { checkPointsReached = value; }
    }
}
