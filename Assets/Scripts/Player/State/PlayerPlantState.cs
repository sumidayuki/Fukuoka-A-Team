using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PlayerPlantState : StateBase<Player>
{
    PlayerPlantSMB m_plantSMB;

    public override void Enter(Player player)
    {
        StageManager.Instance.PlaceBomb(player.PlayerTransform);

        m_plantSMB = player.GetBehaviour<PlayerPlantSMB>();

        player.Anim.SetTrigger("Plant");
    }

    public override void Execute(Player player, InputInfo input)
    {

        if (m_plantSMB.StateEnd)
        {
            player.ChangeState(new PlayerMoveState());
        }

    }

    public override void Exit(Player player)
    {
        player.Anim.ResetTrigger("Plant");
    }
}
