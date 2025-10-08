using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// すべてのデータSOを一元管理するマネージャ
/// </summary>
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("データアセット参照")]
    [SerializeField] PlayerData playerData;
    [SerializeField] BombData bombData;
    [SerializeField] StageData stageData;

    private Dictionary<int, BombRow> bombDict;
    private Dictionary<int, StageRow> stageDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        // 爆弾データの辞書化
        bombDict = new Dictionary<int, BombRow>();
        foreach (var row in bombData.rows)
            bombDict[row.id] = row;

        // ステージデータ
        stageDict = new Dictionary<int, StageRow>();
        foreach (var row in stageData.rows)
            stageDict[row.id] = row;
    }

    /// <summary>
    /// PlayerData を取得します。
    /// </summary>
    /// <returns></returns>
    public PlayerData GetPlayerData() { return playerData; }

    /// <summary>
    /// BombData から指定の爆弾を取得します。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BombRow GetBomb(int id)
    {
        bombDict.TryGetValue(id, out var row);
        return row;
    }

    /// <summary>
    /// StageData から指定のステージ情報を取得します。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public StageRow GetStage(int id)
    {
        stageDict.TryGetValue(id, out var row);
        return row;
    }

    /// <summary>
    /// ステージデータを取得します。
    /// </summary>
    /// <returns></returns>
    public StageData GetStageData() { return stageData; }
}
