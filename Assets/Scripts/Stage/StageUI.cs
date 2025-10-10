using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] private Transform m_bombButtonParent;
    [SerializeField] private GameObject m_bombButtonPrefab;
    [SerializeField] private Image m_bombUI;
    [SerializeField] private GameObject m_gameOverPanel;
    [SerializeField] private GameObject m_gameClearPanel;
    private Dictionary<int, Button> m_bombButtonDict = new Dictionary<int, Button>();

    private const int CellSize = 25;

    public IEnumerator CreateBombButtons(Dictionary<int, int> bombInventory)
    {
        m_gameOverPanel.SetActive(false);

        // 既存ボタン削除
        foreach (Transform child in m_bombButtonParent)
        {
            Destroy(child.gameObject);
            yield return null;
        }

        m_bombButtonDict.Clear();

        foreach (var bomb in bombInventory)
        {
            int bombId = bomb.Key;
            int count = bomb.Value;

            BombRow bombRow = DataManager.Instance.GetBomb(bombId);

            GameObject btnObj = Instantiate(m_bombButtonPrefab, m_bombButtonParent);
            Button btn = btnObj.GetComponent<Button>();
            Text txt = btnObj.GetComponentInChildren<Text>();

            Image imageParent = btnObj.GetComponentInChildren<Image>();
            for(int x = 0; x < bombRow.x.Length; x++)
            {
                for(int i = 0; i < bombRow.x[x]; i++)
                {
                    Vector3 dir = x == 0 ? Vector3.left : Vector3.right;
                    Instantiate(m_bombUI, imageParent.transform.position + (i + 1) * dir * CellSize, Quaternion.identity, imageParent.transform);
                }
            }
            for (int y = 0; y < bombRow.z.Length; y++)
            {
                for (int i = 0; i < bombRow.z[y]; i++)
                {
                    Vector3 dir = y == 0 ? Vector3.up : Vector3.down;
                    Instantiate(m_bombUI, imageParent.transform.position + (i + 1) * dir * CellSize, Quaternion.identity, imageParent.transform);
                }
            }

            txt.text = $"残り:{count}";
            btn.interactable = count > 0;

            btn.onClick.AddListener(() =>
            {
                SelectBomb(bombId);
                HighlightButton(bombId);
            });

            m_bombButtonDict[bombId] = btn;

            yield return null;
        }
    }

    public void UpdateBombButtonState(Dictionary<int, int> bombInventory, int selectedId)
    {
        foreach (var bomb in bombInventory)
        {
            if (m_bombButtonDict.TryGetValue(bomb.Key, out Button btn))
            {
                Text txt = btn.GetComponentInChildren<Text>();
                txt.text = $"残り:{bomb.Value}";
                btn.interactable = bomb.Value > 0;
            }
        }
        HighlightButton(selectedId);
    }

    public void LoadTo(string name)
    {
        GameManager.Instance.LoadTo(name);
    }

    public void SelectBomb(int bombId)
    {
        StageManager.Instance.SetSelectedBombId(bombId);
        Debug.Log($"爆弾 ID:{bombId} を選択");
    }

    private void HighlightButton(int selectedId)
    {
        foreach (var button in m_bombButtonDict)
        {
            Color c = (button.Key == selectedId) ? Color.yellow : Color.white;
            button.Value.image.color = c;
        }
    }

    public void HideButtons()
    {
        m_bombButtonParent.gameObject.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        m_gameOverPanel.SetActive(true);
    }

    public void ShowGameClearPanel()
    {
        m_gameClearPanel.SetActive(true);
    }
}