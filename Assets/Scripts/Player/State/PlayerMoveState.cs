using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : StateBase<Player>
{
    private Rigidbody rb;
    private Transform cam;
    
    private float turnLerp = 10f;

    PlayerMoveSMB m_moveSMB;

    public override void Enter(Player player)
    {
        player.Anim.SetTrigger("Walk");

        m_moveSMB = player.GetBehaviour<PlayerMoveSMB>();
        
        rb = player.Rb;
        cam = Camera.main ? Camera.main.transform : null;

        if (rb != null)
            rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public override void Execute(Player player, InputInfo input)
    {
        if(input.Plant)
        {
            m_moveSMB.PlantInput();
        }

        // 爆弾設置入力が来たら設置ステートへ
        if (m_moveSMB.StateChangePlant)
        {
            player.ChangeState(new PlayerPlantState());
        }
    }

    public override void FixedExecute(Player player, InputInfo input)
    {
        Vector3 inDir = input.Move;
        if (inDir.sqrMagnitude < 0.0001f)
        {
            player.Anim.SetFloat("Speed", 0);
            return;
        }
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
        if (player.PlayerTransform != null)
        {
            Quaternion target = Quaternion.LookRotation(inDir);
            player.PlayerTransform.rotation = Quaternion.Slerp(player.PlayerTransform.rotation, target, turnLerp * Time.deltaTime);
        }

        float currentSpeed = 0;

        if (Mathf.Abs(input.Move.x) != 0 || Mathf.Abs(input.Move.z) != 0)
        {
            currentSpeed = 1;
        }
        else
        {
            currentSpeed = 0;
        }

        player.Anim.SetFloat("Speed", input.Move.magnitude * currentSpeed);
    }

    public override void Exit(Player player)
    {
        player.Anim.ResetTrigger("Walk");
    }
}
