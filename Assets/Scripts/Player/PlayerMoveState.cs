using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMoveState : StateBase<Player>
{
    private Rigidbody rb;
    private Transform model;
    private Transform cam;

    private float walkSpeed = 3.0f;
    private float turnLerp = 10f;

    public override void Enter(Player player)
    {
        base.Enter(player);
        rb = player.Rb;
        model = player.Model;
        cam = Camera.main ? Camera.main.transform : null;

        if (rb)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    public override void Execute(Player character, InputInfo input)
    {
        Vector3 inDir = input.Move;
        inDir.y = 0f;
        if (inDir.sqrMagnitude < 0.0001f) return;
        inDir.Normalize();

        
        if (cam != null)
        {
            Vector3 f = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 r = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            inDir = (f * input.Move.z + r * input.Move.x).normalized;
        }

        Vector3 next = rb.position + inDir * walkSpeed * Time.deltaTime;
        rb.MovePosition(next);

        if (model != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(inDir);
            model.rotation = Quaternion.Slerp(model.rotation, targetRot, turnLerp * Time.deltaTime);
            
        }
    }
}
