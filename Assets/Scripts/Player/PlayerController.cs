using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseUpdate
{
    private Player player;
    private InputInfo input;

    private void Start()
    {
        GameManager.Instance.RegisterSystem(this);
    }
    private void OnDisable()
    {
        GameManager.Instance.UnregisterSystem(this);
    }

    public override void Enter()
    {
        input = InputManager.Instance.Info;
        player = gameObject.AddComponent<Player>();
        player.Enter();
    }

    public override void Execute()
    {
        player.Execute(input);
    }

    public override void FixedExecute()
    {
        player.FixedExecute(input);
    }

    public override void LateExecute()
    {
        player.LateExecute(input);
    }
}
