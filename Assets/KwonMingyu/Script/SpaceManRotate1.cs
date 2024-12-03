using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManRotate1 : MonoBehaviour
{
    [SerializeField] float rotateSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.Self);
    }
}
