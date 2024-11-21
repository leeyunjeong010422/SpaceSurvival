using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement2 : MonoBehaviour
{
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// 중력을 고려하지 않고 플레이어가 이동하려는 거리를 입력
    /// </summary>
    /// <param name="moveVector"></param>
    public void Move(Vector3 moveVector)
    {
        rigid.MovePosition(rigid.position + moveVector);
    }

    public void LookDirection(Vector3 direction)
    {
        rigid.MoveRotation(Quaternion.LookRotation(direction));
    }
}
