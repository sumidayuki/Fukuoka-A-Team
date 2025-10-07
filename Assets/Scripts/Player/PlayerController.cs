using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseUpdate
{
    private Player player;

    public override void Enter()
    {
        player.Enter();
    }

    public override void Execute()
    {
        player.Execute();
    }
}
