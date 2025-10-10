using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Player : MonoBehaviour, IDamageable
{
    public Rigidbody Rb { get; private set; }

    public Transform PlayerTransform { get; private set; }

    public Animator Anim { get; private set; }

    public PlayerCamera Cam { get; private set; }

    // データ由来のパラメータ
    public float MoveSpeed { get; private set; }

    // 内部状態
    private bool _isDead = false;
    private StateManager<Player> _stateManager;
    
    public void Enter()
    {
        MoveSpeed = DataManager.Instance.GetPlayerData().moveSpeed;

        Rb = gameObject.GetComponent<Rigidbody>();

        PlayerTransform = gameObject.transform;

        Anim = gameObject.GetComponent<Animator>();

        Cam = Camera.main.gameObject.AddComponent<PlayerCamera>();
        Cam.SetTarget(StageManager.Instance.GetStageCenter());

        _stateManager = new StateManager<Player>();
        _stateManager.Init(new PlayerMoveState(), this);
    }

    public void Execute(InputInfo input)
    {
        if (_isDead) return;
        _stateManager.CurrentState.Execute(this, input);
    }

    public void FixedExecute(InputInfo input)
    {
        if (_isDead) return;
        _stateManager.CurrentState.FixedExecute(this, input);
    }

    public void LateExecute(InputInfo input)
    {
        if (Cam)
            Cam.CameraUpdate(input);
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

    /// <summary>
    /// AnimatorについているSMBを取得します。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetBehaviour<T>() where T : StateMachineBehaviour
    {
        return Anim.GetBehaviour<T>();
    }

}
