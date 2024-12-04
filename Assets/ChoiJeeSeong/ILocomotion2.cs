using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocomotion2
{
    GameObject gameObject { get; }

    public void SetVelocity(Vector3 velocity);
    public void LookDirection(Vector3 direction);
}
