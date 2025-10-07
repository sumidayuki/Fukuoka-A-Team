using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステートの状態を管理するクラスです。
/// </summary>
/// <typeparam name="T"></typeparam>
public class StageManager<T>
{
    /// <summary>
    /// 現在の状態
    /// </summary>
    public StateBase<T> CurrentState { get; private set; }

    /// <summary>
    /// ステートを変更します。
    /// </summary>
    /// <param name="newState">変更先のステート</param>
    /// <param name="character">指定先のキャラクター</param>
    public void ChangeState(StateBase<T> newState, T character)
    {
        // 現在のステートと変更したいステートが同じだったら
        if (CurrentState == newState)
        {
            Debug.Log("同じステートです");
            return;
        }

        // 現在のステートを終了します。
        CurrentState.Exit(character);

        // 現在のステートに新しいステートを代入します。
        CurrentState = newState;

        // 新しいステートを実行します。
        newState.Enter(character);
    }

    /// <summary>
    /// ステートの初期化をします。
    /// </summary>
    /// <param name="newState">初期化先のステート</param>
    /// <param name="character">指定先のキャラクター</param>
    public void Init(StateBase<T> newState, T character)
    {
        // 現在のステートに新しいステートを代入します。
        CurrentState = newState;

        // 現在のステートを実行します。
        CurrentState.Enter(character);
    }
}
