using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    public void Enter();
    public void Execute();
    public void Exit();
}
