using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement2 : MonoBehaviour, ILocomotion2
{
    private Rigidbody rigid;
    private Vector3 velocity;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rigid.MovePosition(rigid.position + Time.deltaTime * velocity);
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void LookDirection(Vector3 direction)
    {
        rigid.MoveRotation(Quaternion.LookRotation(direction));
    }
}
