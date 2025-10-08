using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Rigidbody Rb { get; set; }
    public Transform Model { get; set; }

    private StateManager<Player> stateManager;
    
    public void Enter()
    {
        stateManager = new StateManager<Player>();
        stateManager.Init(new PlayerMoveState(), this);
    }

    public void Execute(InputInfo input)
    {
        stateManager.CurrentState.Execute(this, input);
    }
<<<<<<< HEAD
}
=======

    public void ChangeState(StateBase<Player> newState)
    {
        stateManager.ChangeState(newState, this);
    }

}
>>>>>>> main
