using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveSMB : StateMachineBehaviour
{
    private bool m_canStateChangePlant;

    public bool StateChangePlant { get; private set; }

    /// <summary>
    /// ステートが開始されたときに呼び出されます。
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_canStateChangePlant = false;
        StateChangePlant = false;
    }

    /// <summary>
    /// ステートが有効な間、毎フレーム呼び出されます。
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.IsInTransition(layerIndex))
        {
            if (m_canStateChangePlant)
            {
                StateChangePlant = true;
            }
        }
    }

    /// <summary>
    /// DiveRollが入力された時に呼びだされます。
    /// </summary>
    public void PlantInput()
    {
        m_canStateChangePlant = true;
    }
}
