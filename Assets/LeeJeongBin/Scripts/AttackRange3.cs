using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus.Input;
using UnityEngine;

public class AttackRange3 : MonoBehaviour
{
    [SerializeField] Killing3 killing3;
    private void OnTriggerEnter(Collider other)
    {
        killing3.HandleTargetDeath(other.gameObject);
    }
}
