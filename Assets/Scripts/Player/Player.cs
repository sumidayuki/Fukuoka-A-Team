using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IDamageable
{
    // 外部からセットされる参照(PlayerControllerで代入)
    public Rigidbody Rb { get; set; }
    public Transform Model { get; set; }

    // データ由来のパラメータ
    public float MoveSpeed { get; private set; } = 3.0f;

    // 内部状態
    private bool _isDead = false;
    private StateManager<Player> _stateManager;
    
    public void Enter(PlayerData data = null)
    {
        if (data != null)
            MoveSpeed = data.moveSpeed;

        _stateManager = new StateManager<Player>();
        _stateManager.Init(new PlayerMoveState(), this);
    }

    public void Execute(InputInfo input)
    {
        if (_isDead) return;
        _stateManager.CurrentState.Execute(this, input);
    }

    public void ChangeState(StateBase<Player> newState)
    {
        _stateManager.ChangeState(newState, this);
    }

    public void Damage(float amount)
    {
        if (_isDead) return;

        _isDead = true;
        ChangeState(new PlayerDeadState());
    }

}
