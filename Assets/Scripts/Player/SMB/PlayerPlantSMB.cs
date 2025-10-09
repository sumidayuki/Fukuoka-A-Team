using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlantSMB : StateMachineBehaviour
{
    // 現在の状態が終了するタイミング
    [SerializeField] private float END_START_TIME;

    /// <summary>
    /// ステートが終了しているかを管理します。
    /// </summary>
    public bool StateEnd { get; private set; }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 初期化
        StateEnd = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.IsInTransition(layerIndex))
        {
            var normalizedTime = stateInfo.normalizedTime;

            // このステートの経過時間がステートの終了時間なら
            if (normalizedTime >= END_START_TIME)
            {
                // ステートの終了を伝える
                StateEnd = true;
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
