using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUpdate : MonoBehaviour
{
    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
}
