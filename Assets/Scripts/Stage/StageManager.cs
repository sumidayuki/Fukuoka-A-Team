using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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
    [SerializeField] GameObject m_centerObj;
    private int m_currentStageId = 0;
    private StageRow m_currentStageData;
    private Vector3Int m_stageSize;
    #endregion

    #region 爆弾管理
    [Header("爆弾管理")]
    [SerializeField] private GameObject m_bombPrefab;
    private Dictionary<int, int> m_bombInventoryDict = new Dictionary<int, int>();
    private Dictionary<int, GameObject> m_bombObjectDict = new Dictionary<int, GameObject>();

    private Dictionary<int, Queue<Bomb>> m_bombDict = new Dictionary<int, Queue<Bomb>>();

    private int m_currentBombCount = 0;
    private const int MAX_BOMB_COUNT = 3;
    private int m_selectedBombId = 0;
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
    [SerializeField] private GameObject m_enemyPrefab;
    private GameObject m_player;
    #endregion
    
    #region UI管理
    [Header("UI管理")]
    private StageUI m_stageUI;
    #endregion

    #region カメラ設定
    [Header("カメラ設定")]
    private Vector3 m_cameraOffset;
    private Transform m_cameraPivot;
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
    private const float COUNTDOWN_TIME = 5f;
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

        InitializeCamera();
        InitializeUI();

        Debug.Log("StageManager: ロード完了");
        yield return null;
    }

    private IEnumerator LoadStageData()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string[] parts = sceneName.Split('_');

        if (parts.Length >= 2 && int.TryParse(parts[1], out int stageId))
        {
            m_currentStageId = stageId;
            Debug.Log($"シーン名からステージID取得: {m_currentStageId}");
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
        m_bombDict.Clear();

        if (true /*m_currentStageData.bombIds != null && m_currentStageData.bombCounts != null*/)
        {
            int length = Mathf.Min(m_currentStageData.bombIds.Length, m_currentStageData.bombCounts.Length);

            for (int i = 0; i < length; i++)
            {
                int bombId = m_currentStageData.bombIds[i];
                int count = m_currentStageData.bombCounts[i];

                // 残数を登録
                m_bombInventoryDict[bombId] = count;
                m_bombDict[bombId] = new Queue<Bomb>();

                BombRow data = DataManager.Instance.GetBomb(bombId);

                // BombRowを取得してBombインスタンスを生成
                for (int j = 0; j < count; j++)
                {
                    if (data != null && m_bombPrefab != null)
                    {
                        GameObject obj = Instantiate(m_bombPrefab, transform);

                        Bomb bomb = obj.GetComponent<Bomb>();
                        if (bomb != null)
                        {
                            bomb.SetBomb(data);
                            obj.SetActive(false);
                            m_bombDict[bombId].Enqueue(bomb);
                            Debug.Log($"爆弾 ID:{bombId} Bombインスタンス生成完了");
                        }
                        else
                        {
                            Debug.LogError($"BombプレハブにBombコンポーネントがありません");
                            Destroy(obj);
                        }
                    }
                }

                Debug.Log($"爆弾 ID:{bombId} 個数:{count}");
            }

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

        Debug.Log("FieldRoot内を探索開始...");

        SearchSpawnersRecursive(m_fieldRoot);

        Debug.Log($"探索完了: プレイヤー{(m_player != null ? "生成済み" : "未生成")} / 敵ターゲット数: {m_totalEnemyCount}");
        yield return null;
    }

    private void SearchSpawnersRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag("PlayerSpawner"))
            {
                SpawnPlayer(child.position);
            }
            else if (child.CompareTag("EnemySpawner"))
            {
                SpawnEnemy(child.position);
            }
            else if (child.CompareTag("Enemy"))
            {
                m_enemyBlocks.Add(child.gameObject);
                m_totalEnemyCount++;
                Debug.Log($"エネミーブロック発見: {child.name}");
            }

            SearchSpawnersRecursive(child);
        }
    }

    private void SpawnPlayer(Vector3 position)
    {
        if (m_player != null)
        {
            Debug.LogWarning("プレイヤーは既に生成されています");
            return;
        }

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
        if (m_enemyPrefab != null)
        {
            GameObject enemy = Instantiate(m_enemyPrefab, position, Quaternion.identity);
            Debug.Log($"敵を生成しました 位置: {position}");
        }
        else
        {
            Debug.LogWarning($"敵プレハブが設定されていません。位置: {position}");
        }
    }
    #endregion

    #region BaseUpdate オーバーライド
    public override void Enter()
    {
        Debug.Log("StageManager: Enter");
    }

    public override void Execute()
    {
        CheckPlayerFallDeath();

        if (m_isCountingDown)
        {
            m_countdownTimer -= Time.deltaTime;
            //UpdateUI();

            if (m_countdownTimer <= 0f)
            {
                OnGameOver();

                m_isCountingDown = false;
                CheckDefeatCondition();
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

        if (!m_bombDict.ContainsKey(bombId))
        {
            Debug.LogError($"爆弾 ID:{bombId} のBombインスタンスが見つかりません");
            return false;
        }

        return true;
    }

    public bool PlaceBomb(Transform playerTransform)
    {
        int bombId = m_selectedBombId;

        if (!CanPlaceBomb(bombId))
        {
            Debug.LogWarning($"爆弾 ID:{bombId} を設置できません");
            return false;
        }

        Bomb bomb = m_bombDict[bombId].Dequeue();

        // 爆弾を配置
        bomb.gameObject.SetActive(true);
        bomb.Plant(playerTransform); // 爆弾の設置処理を呼ぶ

        m_bombInventoryDict[bombId]--;
        m_currentBombCount++;

        Debug.Log($"爆弾 ID:{bombId} を設置しました 位置:{playerTransform.position} 残り: {m_bombInventoryDict[bombId]}");

        m_stageUI.UpdateBombButtonState(m_bombInventoryDict, bombId);

        CheckBombExhaustion();

        return true;
    }

    public void OnBombExploded(Bomb bomb)
    {
        if (m_currentBombCount > 0)
        {
            m_currentBombCount--;
        }

        bomb.gameObject.SetActive(false);

        //UpdateUI();
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

    public List<int> GetAvailableBombIds()
    {
        return new List<int>(m_bombInventoryDict.Keys);
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

            //UpdateUI();
            CheckVictoryCondition();
        }
    }

    private void CheckVictoryCondition()
    {
        if (m_destroyedEnemyCount >= m_totalEnemyCount)
        {
            m_isCountingDown = false;
            Debug.Log("ステージクリア！");

            DataManager.Instance.GetPlayerData().unlockedStageCount++;

            m_stageUI.HideButtons();
        }
    }

    public int GetDestroyedEnemyCount()
    {
        return m_destroyedEnemyCount;
    }

    public int GetTotalEnemyCount()
    {
        return m_totalEnemyCount;
    }

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

        if (noBombsLeft && m_currentBombCount == 0 && !m_isCountingDown)
        {
            Debug.Log("爆弾を使い切りました。5秒カウントダウン開始");
            m_isCountingDown = true;
            m_countdownTimer = COUNTDOWN_TIME;
            //UpdateUI();
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

    public bool IsCountingDown()
    {
        return m_isCountingDown;
    }

    public float GetCountdownTime()
    {
        return m_countdownTimer;
    }
    #endregion

    #region プレイヤー死亡判定
    public void OnPlayerDeath()
    {
        Debug.Log("ゲームオーバー: プレイヤーが死亡しました");
        OnGameOver();
    }

    private void CheckPlayerFallDeath()
    {
        if (m_player != null && m_player.transform.position.y < m_fallDeathY)
        {
            Debug.Log("ゲームオーバー: プレイヤーが落下しました");
            OnGameOver();
        }
    }

    private void OnGameOver()
    {
        m_stageUI.HideButtons();
        m_stageUI.ShowGameOverPanel();

        m_isCountingDown = false;

        GameManager.Instance.GameOver();
    }
    #endregion
    
    #region UI管理
    private void InitializeUI()
    {
        m_stageUI = FindObjectOfType<StageUI>();

        if (m_stageUI != null)
        {
            StartCoroutine(m_stageUI.CreateBombButtons(m_bombInventoryDict));
            m_stageUI.UpdateBombButtonState(m_bombInventoryDict, m_selectedBombId);
        }
    }
    #endregion

    #region カメラ設定
    public void InitializeCamera()
    {
        m_centerObj.transform.position = new Vector3(m_stageSize.x / 2f * 2 - 0.5f, m_stageSize.y / 2f * 2 -0.5f, m_stageSize.z / 2f * 2 - 0.5f);

        m_cameraOffset = new Vector3(0, m_stageSize.y / 2.0f, -(m_stageSize.z / 2.0f * 2 * 4));
    }

    public Transform GetStageCenter()
    {
        return m_centerObj.transform;
    }

    public Vector3 GetCameraOffset()
    {
        return m_cameraOffset;
    }

    #endregion

    #region デバッグ用
    public void DebugPrintStatus()
    {
        Debug.Log("===== StageManager 状態 =====");
        Debug.Log($"ステージID: {m_currentStageId}");
        Debug.Log($"ステージサイズ: {m_stageSize}");
        Debug.Log($"エネミー: {m_destroyedEnemyCount}/{m_totalEnemyCount}");
        Debug.Log($"設置中の爆弾: {m_currentBombCount}/{MAX_BOMB_COUNT}");
        Debug.Log($"選択中の爆弾ID: {m_selectedBombId}");
        Debug.Log($"カウントダウン中: {m_isCountingDown}");
        if (m_isCountingDown)
        {
            Debug.Log($"残り時間: {m_countdownTimer:F1}秒");
        }
        Debug.Log("爆弾残数:");
        foreach (var kvp in m_bombInventoryDict)
        {
            Debug.Log($"  ID:{kvp.Key} 個数:{kvp.Value}");
        }
        Debug.Log("==============================");
    }
    #endregion
}