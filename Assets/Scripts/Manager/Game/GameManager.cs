using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BaseUpdates = new List<BaseUpdate>();
        LoadTarget = FindObjectOfType<BaseUpdate>().GetComponent<IManageable>();
    }

    private StateManager<GameManager> m_stateManager;

    // XVˆ—‚ğs‚¤‘ÎÛ‚ğ“o˜^‚µ‚Ä‚¨‚­”z—ñ
    public List<BaseUpdate> BaseUpdates { get; private set; }

    // Load‚·‚éÛ‚ÌIManageable‚ğ•Û
    public IManageable LoadTarget { get; private set; }

    [SerializeField] GameObject m_loadingPanel;

    public GameObject GetLoadingPanel { get { return m_loadingPanel; } }

    private void Start()
    {
        if(LoadTarget != null)
        {
            Debug.Log("IManageable”­Œ©");
        }
        else
        {
            Debug.Log("IManageable‚ªŒ©‚Â‚©‚ç‚È‚©‚Á‚½");
        }

        m_loadingPanel.SetActive(false);

        m_stateManager = new StateManager<GameManager>();

        if (LoadTarget != null)
        {
            m_stateManager.Init(new GameLoadingState(), this);
        }
        else
        {
            m_stateManager.Init(new GamePlayState(), this);

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

    /// <summary>
    /// BaseUpdate ‚ğŒp³‚µ‚Ä‚¢‚éƒNƒ‰ƒX‚ğ“o˜^‚µ‚Ü‚·B
    /// “o˜^‚ğ‚·‚é‚±‚Æ‚Å BaseUpdate ‚©‚çXV‚È‚Ç‚Ìˆ—‚ªŒÄ‚Ño‚³‚ê‚Ü‚·B
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
    /// “o˜^‚³‚ê‚Ä‚¢‚é BaseUpdate ‚ğ‰ğœ‚µ‚Ü‚·B
    /// </summary>
    /// <param name="system"></param>
    public void UnregisterSystem(BaseUpdate system)
    {
        BaseUpdates.Remove(system);
    }
}
