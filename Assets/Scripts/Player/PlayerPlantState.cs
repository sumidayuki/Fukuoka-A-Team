using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PlayerPlantState : StateBase<Player>
{
    public override void Enter(Player player)
    {
        StageManager.Instance.PlaceBomb(player.PlayerTransform);
    }

    public override void Execute(Player player, InputInfo input)
    {
        //player.ChangeState(new PlayerMoveState());
    }
}
