using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : BaseUpdate, IManageable
{
    void Start()
    {
        GameManager.Instance.RegisterSystem(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.UnregisterSystem(this);
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