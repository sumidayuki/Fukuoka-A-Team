using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : BaseUpdate, IManageable
{
    [SerializeField] Transform buttonParent;
    [SerializeField] GameObject buttonPrefab;

    public IEnumerator Load()
    {
        Debug.Log("セレクト");
        yield return StartCoroutine(LoadStageButtons());
    }

    private IEnumerator LoadStageButtons()
    {
        StageData stageData = DataManager.Instance.GetStageData();

        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject); // クリア（再生成時用）
            yield return null;
        }

        PlayerRow playerData = DataManager.Instance.GetPlayerData();

        foreach (var row in stageData.rows)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
            StageButton button = buttonObj.GetComponent<StageButton>();

            button.SetStageInfo(row.name, row.id);

            bool isUnlocked = row.id <= playerData.unlockedStageCount;
            button.gameObject.SetActive(isUnlocked);
            yield return null;
        }
    }

    void Start()
    {
        GameManager.Instance.RegisterSystem(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.UnregisterSystem(this);
    }

    public override void Enter()
    {
    }

    public override void Execute()
    {
    }
}
