using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isStart;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        isStart = false;

        BaseUpdates = new List<BaseUpdate>();

        FindLoadTarget();

        m_stateManager = new StateManager<GameManager>();
        m_stateManager.Init(new GameLoadingState(), this);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private StateManager<GameManager> m_stateManager;

    // 更新処理を行う対象を登録しておく配列
    public List<BaseUpdate> BaseUpdates { get; private set; }

    // Loadする際のIManageableを保持
    public IManageable LoadTarget { get; private set; }

    [SerializeField] GameObject m_loadingPanel;

    public GameObject GetLoadingPanel { get { return m_loadingPanel; } }

    private void Start()
    {
        if(LoadTarget != null)
        {
            Debug.Log("IManageable発見");
        }
        else
        {
            Debug.Log("IManageableが見つからなかった");
        }
    }

    private void Update()
    {
        m_stateManager.CurrentState.Execute(this);
    }

    private void LateUpdate()
    {
        m_stateManager.CurrentState.LateExecute(this);
    }

    private void FixedUpdate()
    {
        m_stateManager.CurrentState.FixedExecute(this);
    }

    public void ChangeState(StateBase<GameManager> newState, GameManager gm)
    {
        m_stateManager.ChangeState(newState, gm);
    }

    public void LoadTo(string nextSceneName)
    {
        m_loadingPanel.SetActive(true);
        LoadTarget = null;
        StartCoroutine(LoadScene(nextSceneName));
    }

    public IEnumerator LoadScene(string nextSceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(nextSceneName);

        while (!async.isDone)
        {
            yield return null;
        }

    }

    /// <summary>
    /// BaseUpdate を継承しているクラスを登録します。
    /// 登録をすることで BaseUpdate から更新などの処理が呼び出されます。
    /// </summary>
    /// <param name="system"></param>
    public void RegisterSystem(BaseUpdate system)
    {
        if(!BaseUpdates.Contains(system))
        {
            BaseUpdates.Add(system);
        }
    }

    /// <summary>
    /// 登録されている BaseUpdate を解除します。
    /// </summary>
    /// <param name="system"></param>
    public void UnregisterSystem(BaseUpdate system)
    {
        BaseUpdates.Remove(system);
    }

    private void FindLoadTarget()
    {
        foreach (var baseUpdate in FindObjectsOfType<BaseUpdate>())
        {
            var manageable = baseUpdate.GetComponent<IManageable>();
            if (manageable != null)
            {
                LoadTarget = manageable;
                break;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isStart) isStart = !isStart;

        Debug.Log($"シーンがロードされました: {scene.name}, モード: {mode}");
        FindLoadTarget();
        ChangeState(new GameLoadingState(), this);
    }
}
