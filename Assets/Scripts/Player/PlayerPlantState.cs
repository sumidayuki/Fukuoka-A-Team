using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PlayerPlantState : StateBase<Player>
{
    private float timer;
    private bool planted;

    // 設置までの待ち時間（アニメ再生タイミング用）
    private const float plantDelay = 0.10f;

    // ステートを抜けるまで（アニメ後戻る想定）
    private const float totalDuration = 0.25f;

    public override void Enter(Player player)
    {
        base.Enter(player);
        timer = 0f;
        planted = false;

        // TODO: 設置アニメ開始など
        Debug.Log("爆弾設置モーション開始");
    }

    public override void Execute(Player player, InputInfo input)
    {
        timer += Time.deltaTime;

        // 設置タイミング
        if (!planted && timer >= plantDelay)
        {
            planted = true;
            
            Debug.Log("爆弾設置！");
        }

        // 設置が終わったら移動状態に戻る
        if (timer >= totalDuration)
        {
            player.ChangeState(new PlayerMoveState());
        } 
    }
}
