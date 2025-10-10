using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : StateBase<Player>
{
    public override void Enter(Player player)
    {
        if (player.Rb != null)
            player.Rb.velocity = Vector3.zero;

        player.Anim.SetTrigger("Death");

        StageManager.Instance.OnPlayerDeath();
    }

    public override void Execute(Player player, InputInfo input)
    {
        // 何もしない（必要ならリトライ入力待ちなど）
    }
}
