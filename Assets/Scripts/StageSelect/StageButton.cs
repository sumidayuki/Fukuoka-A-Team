using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [SerializeField] Text m_stageNameText;
    [SerializeField] Button m_button;

    private int m_stageId;

    public void SetStageInfo(string name, int id)
    {
        m_stageNameText.text = name;
        m_stageId = id;

        m_button.onClick.AddListener(OnClick);
    }

    public void SetInteractable(bool value)
    {
        m_button.interactable = value;
    }

    private void OnClick()
    {
        string sceneName = $"Stage_{m_stageId}";
        GameManager.Instance.LoadTo(sceneName);
    }
}
