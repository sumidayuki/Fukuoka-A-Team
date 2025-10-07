using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private StateManager<Player> statemanager;

    public void Enter()
    {
        statemanager = new StateManager<Player>();
    }

    public void Execute()
    {

    }
}
