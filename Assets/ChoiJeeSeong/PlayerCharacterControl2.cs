using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterControl2 : MonoBehaviourPun
{
    [SerializeField] float speed;

    private Animator animator;
    private int hashSpeed;

    private ILocomotion2 movement;

    private InputAction moveInput;
    private InputAction fireInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<ILocomotion2>();
        if (movement == null)
        {
            Debug.LogError("플레이어 캐릭터에 이동 기능 컴포넌트 누락");
        }

        // 1클라이언트 1플레이어 전제
        PlayerInput input = PlayerInput.GetPlayerByIndex(0);

        moveInput = input.actions["Move"];
        fireInput = input.actions["Fire"];

        hashSpeed = Animator.StringToHash("Speed");
    }

    private void OnEnable()
    {
        // LocalPlayer의 캐릭터가 아니라면 컨트롤 비활성화
        if (false == photonView.IsMine)
        {
            this.enabled = false;
            return;
        }
    }


    private void Update()
    {
        // 이동 입력 처리
        Vector3 inputAxisX = Camera.main.transform.right; // 좌우 입력에 대응하는 월드 방향
        Vector3 inputAxisY = Camera.main.transform.forward; // 전후 입력에 대응하는 월드 방향

        inputAxisX.y = 0f;
        inputAxisY.y = 0f;

        Vector3 cameraLookXZ = inputAxisY;

        inputAxisX.Normalize();
        inputAxisY.Normalize();

        Vector2 inputVector = moveInput.ReadValue<Vector2>();
        Vector3 moveDirection = inputAxisX * inputVector.x + inputAxisY * inputVector.y;
        movement.SetVelocity(speed * moveDirection);

        //// 카메라 정면 방향 바라보기
        //movement.LookDirection(cameraLookXZ);

        // 이동 방향 바라보기
        if (moveDirection != Vector3.zero)
        {
            movement.LookDirection(moveDirection);
        }

        animator.SetFloat(hashSpeed, speed * moveDirection.magnitude);
    }
}
