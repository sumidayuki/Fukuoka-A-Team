using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayState : StateBase<GameManager>
{
    public override void Enter(GameManager gm)
    {
        foreach(var sys in gm.BaseUpdates)
        {
            sys.Enter();
        }
    }

    public override void Execute(GameManager gm)
    {
        foreach (var sys in gm.BaseUpdates)
        {
            sys.Execute();
        }

        if (InputManager.Instance.Info.Pause)
        {
            gm.ChangeState(new GamePauseState(), gm);
        }
    }

    public override void LateExecute(GameManager gm)
    {
        foreach (var sys in gm.BaseUpdates)
        {
            sys.LateExecute();
        }
    }

    public override void FixedExecute(GameManager gm)
    {
        foreach (var sys in gm.BaseUpdates)
        {
            sys.FixedExecute();
        }
    }
}
