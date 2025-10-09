using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMoveState : StateBase<Player>
{
    private Rigidbody rb;
    private Transform model;
    private Transform cam;
    
    private float turnLerp = 10f;

    public override void Enter(Player player)
    {
       
        rb = player.Rb;
        model = player.Model;
        cam = Camera.main ? Camera.main.transform : null;

        if (rb != null)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public override void Execute(Player player, InputInfo input)
    {
        Vector3 inDir = input.Move;
        if (inDir.sqrMagnitude < 0.0001f) return;
        inDir.Normalize();

        // カメラ基準で水平移動
        if (cam != null)
        {
            Vector3 f = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 r = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            inDir = f * inDir.z + r * inDir.x;
            inDir.Normalize();
        }

        // 位置更新(速度はPlayerData由来)
        Vector3 next = rb.position + inDir * player.MoveSpeed * Time.deltaTime;
        rb.MovePosition(next);

        // 向き
        if (model != null)
        {
            Quaternion target = Quaternion.LookRotation(inDir);
            model.rotation = Quaternion.Slerp(model.rotation, target, turnLerp * Time.deltaTime);
        }

        // 爆弾設置入力が来たら設置ステートへ
        if (input.Plant)
        {
            player.ChangeState(new PlayerPlantState());
        }
    }
}
