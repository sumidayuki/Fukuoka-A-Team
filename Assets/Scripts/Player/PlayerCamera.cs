using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : BaseUpdate
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -5);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotateSpeed = 3f;

    private float yaw = 0f;
    private float pitch = 20f;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterSystem(this);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterSystem(this);
    }

    public override void Enter()
    {
        
    }

    public override void Execute()
    {
        if (target == null) return;

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            yaw += mouseX * rotateSpeed;
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desired = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * followSpeed);
        transform.LookAt(target);
    }

    public void SetTarget(Transform t) => target = t;
    
}
