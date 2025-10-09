using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private int m_currentStageId = 0;
    private StageRow m_currentStageData;
    private Vector3Int m_stageSize;

    //private GameObject[,,] m_grid;
    #endregion

    #region 爆弾管理
    [Header("爆弾管理")]
    private Dictionary<int, int> m_bombInventoryDict = new Dictionary<int, int>();
    private int m_currentBombCount = 0;
    private const int MAX_BOMB_COUNT = 3;
    private int m_selectedBombId = 0; // 現在選択中の爆弾ID
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

    #region FieldRoot
    [Header("FieldRoot")]
    [SerializeField] private Transform m_fieldRoot;
    #endregion

    #region カウントダウン
    [Header("カウントダウン")]
    private bool m_isCountingDown = false;
    private float m_countdownTimer = 5f;
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
        yield return StartCoroutine(FindSpawnersAndGenerate());

        SetupCameraPivot();

        Debug.Log("StageManager: ロード完了");
        yield return null;
    }

    private IEnumerator LoadStageData()
    {
        //シーン名からステージID取得
        string sceneName = SceneManager.GetActiveScene().name;  // "Stage_0"
        string[] parts = sceneName.Split('_');                  // ["Stage", "0"]

        if (parts.Length >= 2 && int.TryParse(parts[1], out int stageId))
        {
            m_currentStageId = stageId;
        }
        else
        {
            Debug.LogError($"シーン名からステージIDを取得できませんでした: {sceneName}");
            yield break;
        }

        if (DataManager.Instance != null)
        {
            m_currentStageData = DataManager.Instance.GetStage(m_currentStageId);

            if (m_currentStageData != null)
            {
                //size配列からVector3Intに変換
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

            //最初の爆弾を選択状態に
            if (length > 0)
            {
                m_selectedBombId = m_currentStageData.bombIds[0];
            }
        }
    }
    #endregion

    #region ステージ生成
    private IEnumerator FindSpawnersAndGenerate()
    {
        if (m_fieldRoot == null)
        {
            Debug.LogError("FieldRootが設定されていません");
            yield break;
        }

        //FieldRoot配下の全オブジェクトを探索
        foreach (Transform child in m_fieldRoot.GetComponentsInChildren<Transform>(true))
        {
            //PlayerSpawner検出
            if (child.CompareTag("PlayerSpawner"))
            {
                SpawnPlayer(child.position);
            }
            //EnemySpawner検出
            else if (child.CompareTag("EnemySpawner"))
            {
                SpawnEnemy(child.position);
            }
            //EnemyBlock（ターゲット）検出
            else if (child.CompareTag("Enemy"))
            {
                m_enemyBlocks.Add(child.gameObject);
                m_totalEnemyCount++;
            }
        }

        Debug.Log($"プレイヤー生成完了 / 敵ターゲット数: {m_totalEnemyCount}");
        yield return null;
    }

    private void SpawnPlayer(Vector3 position)
    {
        if (m_playerPrefab != null)
        {
            m_player = Instantiate(m_playerPrefab, position, Quaternion.identity);
            Debug.Log($"プレイヤーを生成しました 位置: {position}");
        }
        else
        {
            Debug.LogError("プレイヤープレハブが設定されていません");
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        Debug.Log($"敵スポーン位置: {position}");
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
    //SpawnPlayerはFindSpawnersAndGenerate内で実装
    #endregion

    #region 敵生成
    //SpawnEnemyはFindSpawnersAndGenerate内で実装
    #endregion

    #region BaseUpdate オーバーライド
    public override void Enter()
    {
        Debug.Log("StageManager: Enter");
    }

    public override void Execute()
    {
        //CheckPlayerFallDeath();

        //カウントダウン処理
        if (m_isCountingDown)
        {
            m_countdownTimer -= Time.deltaTime;
            Debug.Log($"カウントダウン: {m_countdownTimer:F1}秒");

            if (m_countdownTimer <= 0f)
            {
                m_isCountingDown = false;
                CheckDefeatCondition(); //時間切れ後の判定
            }
        }
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

    public int GetSelectedBombId()
    {
        return m_selectedBombId;
    }

    public void SetSelectedBombId(int bombId)
    {
        if (m_bombInventoryDict.ContainsKey(bombId))
        {
            m_selectedBombId = bombId;
            Debug.Log($"爆弾 ID:{bombId} を選択しました");
        }
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

    public bool PlaceBomb(int bombId, Vector3 position)
    {
        if (!CanPlaceBomb(bombId))
        {
            Debug.LogWarning($"爆弾 ID:{bombId} を設置できません");
            return false;
        }

        m_bombInventoryDict[bombId]--;
        m_currentBombCount++;
        Debug.Log($"爆弾 ID:{bombId} を設置しました 残り: {m_bombInventoryDict[bombId]}");

        CheckBombExhaustion();

        return true;
    }

    public void OnBombExploded()
    {
        if (m_currentBombCount > 0)
        {
            m_currentBombCount--;
        }

        //爆弾を使い切ったらカウントダウン開始
        CheckBombExhaustion();
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
                GameManager.Instance.GameClear();
            }
        }
    }

    /// <summary>
    ///爆弾を使い切ったかチェックし、カウントダウン開始
    /// </summary>
    private void CheckBombExhaustion()
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

        //爆弾が残っておらず、設置中の爆弾もない
        if (noBombsLeft && m_currentBombCount == 0 && !m_isCountingDown)
        {
            Debug.Log("爆弾を使い切りました。5秒カウントダウン開始");
            m_isCountingDown = true;
            m_countdownTimer = 5f;
        }
    }

    private void CheckDefeatCondition()
    {
        if (m_destroyedEnemyCount < m_totalEnemyCount)
        {
            Debug.Log("ゲームオーバー: 時間内に敵を倒せませんでした");
            OnGameOver();
        }
    }
    #endregion

    #region プレイヤー死亡判定
    public void OnPlayerDeath()
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
            GameManager.Instance.GameOver();
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
        Debug.Log($"選択中の爆弾: ID {m_selectedBombId}");
        Debug.Log("爆弾残数:");
        foreach (var kvp in m_bombInventoryDict)
        {
            Debug.Log($"  ID:{kvp.Key} 個数:{kvp.Value}");
        }
        Debug.Log("==============================");
    }
    #endregion
}