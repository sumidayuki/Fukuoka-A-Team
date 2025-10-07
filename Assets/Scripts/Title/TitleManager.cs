using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : BaseUpdate, IManageable
{
    public void Start()
    {
        if(GameManager.Instance != null)
        GameManager.Instance.RegisterSystem(this);
    }

    public IEnumerator Load()
    {
        Debug.Log("Load");
        yield return null;
    }

    public override void Enter()
    {
    }

    public override void Execute()
    {
    }
}
