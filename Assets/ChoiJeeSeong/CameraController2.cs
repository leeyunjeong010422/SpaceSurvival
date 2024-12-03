using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController2 : MonoBehaviour
{
    [field: SerializeField] public CameraSetting2 setting;
    [field: SerializeField] public Transform Target { get; set; }

    private InputAction lookInput;
    float verticalCameraAngle;
    float horizontalCameraAngle;

    private void Awake()
    {
        lookInput = PlayerInput.GetPlayerByIndex(0).actions["Look"];
    }

    private void Update()
    {
        // 마우스 이동에 따른 카메라 회전
        Vector2 cursorDelta = lookInput.ReadValue<Vector2>();

        horizontalCameraAngle += cursorDelta.x * setting.CameraSensitivity.x;
        verticalCameraAngle += cursorDelta.y * setting.CameraSensitivity.y;
        verticalCameraAngle = Mathf.Clamp(verticalCameraAngle, setting.MinVerticalAngle, setting.MaxVerticalAngle); // 수직 각도 한계

        transform.rotation = Quaternion.Euler(-verticalCameraAngle, horizontalCameraAngle, 0f);
    }

    private void LateUpdate()
    {
        if (Target == null)
            return;

        Vector3 worldSpaceAt = Target.transform.TransformPoint(setting.LookAt);
        Vector3 worldSpaceFrom = worldSpaceAt - this.transform.TransformVector(setting.LookFrom);

        transform.position = worldSpaceFrom;
    }
}
