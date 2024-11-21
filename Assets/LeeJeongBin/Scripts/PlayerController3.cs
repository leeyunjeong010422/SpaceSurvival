using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController3 : MonoBehaviourPun
{
    [Header("플레이어 이동 설정")]
    [SerializeField] float moveSpeed;            // 이동 속도
    [SerializeField] float rotationSpeed;       // 회전 속도
    [SerializeField] float deceleration;         // 감속 값
    [SerializeField] CharacterController characterController;  // 캐릭터 컨트롤러
    [SerializeField] Vector3 velocity;                // 캐릭터 속도

    private PhotonTransformView photonTransformView;  // PhotonTransformView 컴포넌트

    void Start()
    {
        // 캐릭터 컨트롤러 초기화
        characterController = GetComponent<CharacterController>();

        // PhotonTransformView 컴포넌트 초기화
        photonTransformView = GetComponent<PhotonTransformView>();
    }

    void Update()
    {
        // 로컬 플레이어만 움직이게 처리
        if (photonView.IsMine)
        {
            HandleMovement();
        }

        // PhotonTransformView를 사용하여 동기화할 수 있도록 업데이트
        if (photonTransformView != null)
        {
            photonTransformView.SetSynchronizedValues(velocity.x, velocity.y, velocity.z);
        }
    }

    private void HandleMovement()
    {
        // 입력값 받기 (WASD 또는 방향키)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 입력 방향 벡터 (이동 방향)
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 입력이 있을 때만 이동 처리
        if (direction.magnitude >= 0.1f)
        {
            // 목표 각도 계산 (이동 방향에 맞는 회전 각도)
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            // 현재 각도에서 목표 각도로 부드럽게 회전
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            // 회전 적용
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 이동 방향을 계산하여 이동 벡터 생성
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // 이동 처리 (속도 * 시간)
            velocity.x = moveDirection.x * moveSpeed;
            velocity.z = moveDirection.z * moveSpeed;
        }
        else
        {
            // 이동하지 않을 때는 감속 적용 (서서히 속도를 0으로 감속)
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, 0, deceleration * Time.deltaTime);
        }

        // 바닥에 있을 때는 y 방향 속도를 0으로 설정 (중력 효과를 적용할 준비)
        if (characterController.isGrounded)
        {
            velocity.y = -0.5f;  // 바닥에 있는 경우 약간의 하강 속도를 적용 (중력 효과)
        }
        else
        {
            // 중력 적용 (y 축에 중력 값 추가)
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }

        // 캐릭터 이동 (y축 포함한 속도 이동)
        characterController.Move(velocity * Time.deltaTime);
    }
}
