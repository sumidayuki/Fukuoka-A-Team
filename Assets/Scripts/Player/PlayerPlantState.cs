using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PlayerPlantState : StateBase<Player>
{
    public override void Enter(Player player)
    {
        // TODO: 爆弾プレース (StageManagerを経由)
        // 例: BombPlacer.Place(currentCell, selectedBomb);
    }

    public override void Execute(Player player, InputInfo input)
    {
        player.ChangeState(new PlayerMoveState());
    }
}
