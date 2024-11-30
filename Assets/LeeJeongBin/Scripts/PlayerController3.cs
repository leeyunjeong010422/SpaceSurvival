using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.InputSystem.Processors;

public class PlayerController3 : MonoBehaviourPun
{
    [SerializeField] float moveSpeed;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 velocity;

    private Camera playerCamera;
    private DeathCamera3 deathCamera;
    private LastManAudio lastManAudio;
    private CharacterController characterController;
    private PhotonTransformView photonTransformView;
    private Animator animator;

    [SerializeField] int checkPointsReached;

    public List<CheckPoint3> visitedCheckPoint = new List<CheckPoint3>();

    public LastManScore1 score;

    private bool dead;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        photonTransformView = GetComponent<PhotonTransformView>();
        animator = GetComponent<Animator>();
        lastManAudio = FindObjectOfType<LastManAudio>();
        GameOver3.Instance.OnPlayerSpawn(this);

        score = FindObjectOfType<LastManScore1>();

        if (photonView.IsMine)
        { 
            playerCamera = Camera.main;
            Camera.main.GetComponent<CameraController2>().Target = this.transform;
        }
    }

    void Update()
    {
        if (photonView.IsMine && !dead)
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

        // 카메라 기준으로 앞과 오른쪽 방향 계산
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0f; // Y축 회전 제거 (수평 방향만 고려)
        right.y = 0f;

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        float currentSpeed = moveSpeed;

        // Shift 키를 누르면 스프린트 활성화
        if (Input.GetKey(KeyCode.LeftShift) && moveDirection.magnitude > 0) // 모든 방향 가속
        {
            currentSpeed *= sprintMultiplier; // Shift로 달리기 효과
        }

        if (moveDirection.magnitude >= 0.1f)
        {
            // 이동 방향으로 회전
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 이동 벡터 계산
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
            velocity.y += Physics.gravity.y * Time.deltaTime; // 중력 적용
        }

        characterController.Move(velocity * Time.deltaTime); // 캐릭터 이동
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
        lastManAudio.TriggerCheckPointRPC();
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

    public void DeadPlayer()
    {
        transform.GetComponent<Animator>().SetTrigger("Die4");
        if (dead || !photonView.IsMine) return;
        transform.GetComponent<Killing3>().enabled = false;
        dead = true;
        GameOver3.Instance?.OnPlayerDeath(this);
        StartCoroutine(PlayerDestroy());

    }
    private IEnumerator PlayerDestroy()
    {
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Destroy(gameObject);

      // DeathCamera3 호출용
      //  if (deathCamera != null)
      //  {
      //      deathCamera.OnPlayerDead();
      //  }
    }
}
