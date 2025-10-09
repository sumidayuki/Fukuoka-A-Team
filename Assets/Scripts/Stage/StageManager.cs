using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///ステージ全体を管理するマネージャークラス
/// </summary>
public class StageManager : BaseUpdate, IManageable
{
    #region Singleton
    public static StageManager Instance { get; private set; }
    #endregion

    #region ステージデータ
    [Header("ステージデータ")]
    [SerializeField] private int m_currentStageId = 0;
    private StageRow m_currentStageData;
    private Vector3Int m_stageSize;

    //private GameObject[,,] m_grid;
    #endregion

    #region 爆弾管理
    [Header("爆弾管理")]
    private Dictionary<int, int> m_bombInventoryDict = new Dictionary<int, int>();
    private int m_currentBombCount = 0;
    private const int MAX_BOMB_COUNT = 3;
    #endregion

    #region エネミー管理
    [Header("エネミー管理")]
    private List<GameObject> m_enemyBlocks = new List<GameObject>();
    private int m_totalEnemyCount = 0;
    private int m_destroyedEnemyCount = 0;
    #endregion

    #region プレハブ参照
    [Header("プレハブ")]
    [SerializeField] private GameObject m_playerPrefab;
    [SerializeField] private GameObject m_normalBlockPrefab;
    [SerializeField] private GameObject m_destroyableBlockPrefab;
    [SerializeField] private GameObject m_slopePrefab;
    [SerializeField] private GameObject m_enemyBlockPrefab;
    [SerializeField] private GameObject m_enemyPrefab;
    private GameObject m_player;
    #endregion

    #region カメラ設定
    [Header("カメラ設定")]
    [SerializeField] private Transform m_cameraPivot;
    #endregion

    #region 落下死判定
    [Header("落下死判定")]
    [SerializeField] private float m_fallDeathY = -10f;
    #endregion

    #region 初期化
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterSystem(this);
        }
    }

    public IEnumerator Load()
    {
        Debug.Log("StageManager: ロード開始");

        yield return StartCoroutine(LoadStageData());
        yield return StartCoroutine(GenerateStage());
        yield return StartCoroutine(SpawnPlayer());
        yield return StartCoroutine(SpawnEnemies());

        SetupCameraPivot();

        Debug.Log("StageManager: ロード完了");
        yield return null;
    }

    private IEnumerator LoadStageData()
    {
        if (GameManager.Instance != null)
        {
            // m_currentStageId = GameManager.Instance.GetCurrentStageId();
        }

        if (DataManager.Instance != null)
        {
            m_currentStageData = DataManager.Instance.GetStage(m_currentStageId);

            if (m_currentStageData != null)
            {
                // size配列からVector3Intに変換
                if (m_currentStageData.size != null && m_currentStageData.size.Length >= 3)
                {
                    m_stageSize = new Vector3Int(
                        m_currentStageData.size[0],
                        m_currentStageData.size[1],
                        m_currentStageData.size[2]
                    );
                }

                InitializeBombInventory();
                Debug.Log($"ステージ {m_currentStageId} を読み込みました サイズ: {m_stageSize}");
            }
            else
            {
                Debug.LogError($"ステージデータ ID:{m_currentStageId} が見つかりません");
            }
        }

        yield return null;
    }

    private void InitializeBombInventory()
    {
        m_bombInventoryDict.Clear();

        if (m_currentStageData == null) return;

        //bombIdsとbombCountsは配列
        if (m_currentStageData.bombIds != null && m_currentStageData.bombCounts != null)
        {
            int length = Mathf.Min(m_currentStageData.bombIds.Length, m_currentStageData.bombCounts.Length);

            for (int i = 0; i < length; i++)
            {
                int bombId = m_currentStageData.bombIds[i];
                int count = m_currentStageData.bombCounts[i];
                m_bombInventoryDict[bombId] = count;

                Debug.Log($"爆弾 ID:{bombId} 個数:{count}");
            }
        }
    }
    #endregion

    #region ステージ生成
    private IEnumerator GenerateStage()
    {
        Debug.Log("ステージ生成中...");

        //スポナーを使ってブロックを配置（未着手）

        //m_grid = new GameObject[m_stageSize.x, m_stageSize.y, m_stageSize.z];

        /*
        for(int x = 0; x < m_stageSize.x; x++)
        {
            for(int y = 0; y < m_stageSize.y; y++)
            {
                for(int z = 0; z < m_stageSize.z; z++)
                {
                    SpawnBlock(x, y, z, blockType);
                }
            }
        }
        */

        yield return null;
    }

    private void SpawnBlock(int x, int y, int z, string blockType)
    {
        Vector3 position = new Vector3(x, y, z);
        GameObject prefab = null;

        switch (blockType)
        {
            case "Normal":
                prefab = m_normalBlockPrefab;
                break;
            case "Destroyable":
                prefab = m_destroyableBlockPrefab;
                break;
            case "Slope":
                prefab = m_slopePrefab;
                break;
            case "Enemy":
                prefab = m_enemyBlockPrefab;
                m_totalEnemyCount++;
                break;
        }

        if (prefab != null)
        {
            GameObject block = Instantiate(prefab, position, Quaternion.identity, transform);

            if (blockType == "Enemy")
            {
                m_enemyBlocks.Add(block);
            }

            //m_grid[x, y, z] = block;
        }
    }
    #endregion

    #region プレイヤー生成
    private IEnumerator SpawnPlayer()
    {
        if (m_playerPrefab != null)
        {
            //プレイヤー生成ブロックの位置を取得？（あとでやる）
            Vector3 spawnPosition = Vector3.zero;

            m_player = Instantiate(m_playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("プレイヤーを生成しました 位置: " + spawnPosition);
        }
        else
        {
            Debug.LogError("プレイヤープレハブが設定されていません");
        }

        yield return null;
    }
    #endregion

    #region 敵生成
    private IEnumerator SpawnEnemies()
    {
        Debug.Log("敵を生成中...");

        //敵の生成をどうするか考え中

        /*
        if(m_enemyPrefab != null)
        {
            Vector3 enemyPosition = new Vector3(1, 2, 3);
            GameObject enemy = Instantiate(m_enemyPrefab, enemyPosition, Quaternion.identity, transform);
        }
        */

        yield return null;
    }
    #endregion

    #region BaseUpdate オーバーライド
    public override void Enter()
    {
        Debug.Log("StageManager: Enter");
    }

    public override void Execute()
    {
        // CheckPlayerFallDeath();
    }

    public override void LateExecute()
    {
    }

    public override void FixedExecute()
    {
    }
    #endregion

    #region 爆弾管理API
    public int GetBombCount(int bombId)
    {
        if (m_bombInventoryDict.ContainsKey(bombId))
        {
            return m_bombInventoryDict[bombId];
        }
        return 0;
    }

    public int GetCurrentBombCount()
    {
        return m_currentBombCount;
    }

    public bool CanPlaceBomb(int bombId)
    {
        if (m_currentBombCount >= MAX_BOMB_COUNT)
        {
            Debug.Log("同時設置数が上限です");
            return false;
        }

        if (!m_bombInventoryDict.ContainsKey(bombId) || m_bombInventoryDict[bombId] <= 0)
        {
            Debug.Log($"爆弾 ID:{bombId} の残数がありません");
            return false;
        }

        return true;
    }

    public void PlaceBomb(int bombId)
    {
        if (CanPlaceBomb(bombId))
        {
            m_bombInventoryDict[bombId]--;
            m_currentBombCount++;
            Debug.Log($"爆弾 ID:{bombId} を設置しました 残り: {m_bombInventoryDict[bombId]}");
        }
    }

    public void OnBombExploded()
    {
        m_currentBombCount--;
        if (m_currentBombCount < 0) m_currentBombCount = 0;

        CheckDefeatCondition();
    }

    public BombRow GetBombData(int bombId)
    {
        if (DataManager.Instance != null)
        {
            return DataManager.Instance.GetBomb(bombId);
        }
        return null;
    }
    #endregion

    #region エネミー破壊管理
    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (m_enemyBlocks.Contains(enemy))
        {
            m_enemyBlocks.Remove(enemy);
            m_destroyedEnemyCount++;

            Debug.Log($"エネミーを破壊しました: {m_destroyedEnemyCount}/{m_totalEnemyCount}");

            CheckVictoryCondition();
        }
    }

    private void CheckVictoryCondition()
    {
        if (m_destroyedEnemyCount >= m_totalEnemyCount)
        {
            Debug.Log("ステージクリア！");
            if (GameManager.Instance != null)
            {
                //GameManager.Instance.ChangeState(new GameClearState(), GameManager.Instance);
            }
        }
    }

    private void CheckDefeatCondition()
    {
        bool noBombsLeft = true;
        foreach (var count in m_bombInventoryDict.Values)
        {
            if (count > 0)
            {
                noBombsLeft = false;
                break;
            }
        }

        if (noBombsLeft && m_currentBombCount == 0 && m_destroyedEnemyCount < m_totalEnemyCount)
        {
            Debug.Log("ゲームオーバー: 爆弾がなくなりました");
            OnGameOver();
        }
    }
    #endregion

    #region プレイヤー死亡判定
    public void OnPlayerDaeth()
    {
        Debug.Log("ゲームオーバー: プレイヤーが爆発に巻き込まれました");
        OnGameOver();
    }

    // public void OnPlayerHitByExplosion()
    // {
    //     Debug.Log("ゲームオーバー: プレイヤーが爆発に巻き込まれました");
    //     OnGameOver();
    // }
    // 
    // private void CheckPlayerFallDeath()
    // {
    //     if (m_player != null && m_player.transform.position.y < m_fallDeathY)
    //     {
    //         Debug.Log("ゲームオーバー: プレイヤーが落下しました");
    //         OnGameOver();
    //     }
    // }

    private void OnGameOver()
    {
        if (GameManager.Instance != null)
        {
            //GameManager.Instance.ChangeState(new GameOverState(), GameManager.Instance);
        }
    }
    #endregion

    #region カメラ設定
    private void SetupCameraPivot()
    {
        if (m_cameraPivot != null)
        {
            Vector3 stageCenter = new Vector3(
                m_stageSize.x / 2f,
                m_stageSize.y / 2f,
                m_stageSize.z / 2f
            );

            m_cameraPivot.position = stageCenter;
            Debug.Log($"カメラピボットをステージ中心に設定: {stageCenter}");
        }
    }

    public Vector3 GetStageCenter()
    {
        return new Vector3(
            m_stageSize.x / 2f,
            m_stageSize.y / 2f,
            m_stageSize.z / 2f
        );
    }
    #endregion

    #region デバッグ用
    public void DebugPrintStatus()
    {
        Debug.Log("---- StageManager 状態 ----");
        Debug.Log($"ステージID: {m_currentStageId}");
        Debug.Log($"ステージサイズ: {m_stageSize}");
        Debug.Log($"エネミー: {m_destroyedEnemyCount}/{m_totalEnemyCount}");
        Debug.Log($"設置中の爆弾: {m_currentBombCount}/{MAX_BOMB_COUNT}");
        Debug.Log("爆弾残数:");
        foreach (var kvp in m_bombInventoryDict)
        {
            Debug.Log($"  ID:{kvp.Key} 個数:{kvp.Value}");
        }
        Debug.Log("==============================");
    }
    #endregion
}