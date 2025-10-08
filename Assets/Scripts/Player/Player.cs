using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private StateManager<Player> statemanager;

    private CameraOrbit_MouseDrag camera;
    
    public Rigidbody Rb;
    public Transform Model;
    

    public void Enter()
    {
        statemanager = new StateManager<Player>();
        
    }

    public void Execute()
    {

    }
}