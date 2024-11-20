using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController4 : MonoBehaviour
{
    [SerializeField] float speed;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3();
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");

        if (moveDir == Vector3.zero)
            return;

        Vector3 worldMoveDir = transform.TransformDirection(moveDir).normalized;
        transform.position += worldMoveDir * speed * Time.deltaTime;
    }
}
