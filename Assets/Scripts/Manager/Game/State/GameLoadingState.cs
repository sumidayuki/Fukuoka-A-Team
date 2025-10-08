using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoadingState : StateBase<GameManager>
{
    private bool isLoaded;

    private IEnumerator Load(GameManager gm)
    {
        if (gm.LoadTarget != null)
        {
            Debug.Log("Load");
            yield return gm.LoadTarget.Load();
        }
        else
        {
            yield return null;
        }

        isLoaded = true;
    }

    public override void Enter(GameManager gm)
    {
        isLoaded = false;
        gm.GetLoadingPanel.SetActive(true);
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
        gm.GetLoadingPanel.SetActive(false);
    }
}
