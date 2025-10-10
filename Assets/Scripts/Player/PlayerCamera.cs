using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : BaseUpdate
{
    public Transform Target { get; set; }
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotateSpeed = 3f;

    private float yaw = 0f;
    private float pitch = 20f;


    public void CameraUpdate(InputInfo input)
    {
        if (Target == null) return;

        if (input.RightClick)
        {
            float mouseX = input.Look.x;
            yaw += mouseX * rotateSpeed / 10;

            float mouseY = input.Look.y;
            pitch = Mathf.Clamp(pitch - mouseY, -10, 20);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desired = Target.position + rotation * StageManager.Instance.GetCameraOffset();

        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * followSpeed);
        transform.LookAt(Target);
    }

    public void SetTarget(Transform t) => Target = t;

}
