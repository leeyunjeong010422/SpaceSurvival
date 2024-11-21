using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IcySlidingMovement2 : MonoBehaviour, ILocomotion2
{
    [SerializeField] float forceMultiplier = 0.2f;

    private Rigidbody rigid;
    private Vector3 forceVector;
    private float forceMagnitued;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigid.AddForce(forceMultiplier * forceVector, ForceMode.Force);
    }

    public void SetVelocity(Vector3 velocity)
    {
        forceVector = velocity;
        forceMagnitued = forceVector.magnitude;
    }

    public void LookDirection(Vector3 direction)
    {
        rigid.MoveRotation(Quaternion.LookRotation(direction));
    }
}
