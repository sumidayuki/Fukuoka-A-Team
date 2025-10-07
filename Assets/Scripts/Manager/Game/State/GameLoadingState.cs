using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoadingState : StateBase<GameManager>
{
    private bool isLoaded;

    private IEnumerator Load(GameManager gm)
    {
        yield return gm.LoadTarget.Load();

        isLoaded = true;
    }
    
    public override void Enter(GameManager gm)
    {
        isLoaded = false;
        gm.StartCoroutine(Load(gm));
    }

    public override void Execute(GameManager gm)
    {
        if(isLoaded)
        {
            gm.ChangeState(new GamePlayState(), gm);
        }
    }

    public override void Exit(GameManager gm)
    {
    }
}
