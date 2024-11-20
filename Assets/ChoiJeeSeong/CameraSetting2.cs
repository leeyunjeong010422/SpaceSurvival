using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Camera Setting")]
public class CameraSetting2 : ScriptableObject
{
    [SerializeField, Tooltip("대상의 좌표공간 기준 바라볼 위치(ex 머리 위치)")] Vector3 lookAt = Vector3.zero;
    public Vector3 LookAt => lookAt;

    [SerializeField, Tooltip("카메라 좌표공간 기준 lookAt지점 위치")] Vector3 lookFrom = Vector3.zero;
    public Vector3 LookFrom => lookFrom; 

    [SerializeField] Vector2 cameraSensitivity = Vector3.one * 0.3f;
    public Vector2 CameraSensitivity => cameraSensitivity;

    [SerializeField] float maxVerticalAngle = 60f;
    public float MaxVerticalAngle => maxVerticalAngle;

    [SerializeField] float minVerticalAngle = -60f;
    public float MinVerticalAngle => minVerticalAngle;
}
