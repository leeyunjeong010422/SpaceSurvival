using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController3 : MonoBehaviour
{
    [SerializeField] float sensitivityX;
    [SerializeField] float sensitivityY;
    [SerializeField] float minYAngle;
    [SerializeField] float maxYAngle;
    [SerializeField] Transform player;
    [SerializeField] Camera playerCamera;

    private float rotationX = 0f;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleCameraRotation();
        }
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX -= mouseY * sensitivityY;
        rotationX = Mathf.Clamp(rotationX, minYAngle, maxYAngle);

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        player.Rotate(Vector3.up * mouseX * sensitivityX);
    }
}
