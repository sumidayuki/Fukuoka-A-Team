using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviourのStart、Update等の処理を一元管理するための基底クラスです。
/// </summary>
public abstract class BaseUpdate : MonoBehaviour
{
    /// <summary>
    /// Start() の代わりとなる関数です。
    /// </summary>
    public virtual void Enter() { }
    
    /// <summary>
    /// Update() の代わりとなる関数です。
    /// </summary>
    public virtual void Execute() { }

    /// <summary>
    /// LateUpdate() の代わりとなる関数です。
    /// </summary>
    public virtual void LateExecute() { }

    /// <summary>
    /// FixedUpdate() の代わりとなる関数です。
    /// </summary>
    public virtual void FixedExecute() { }
}
