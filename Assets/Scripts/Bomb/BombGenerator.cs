using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombGenerator : MonoBehaviour
{
    [SerializeField] GameObject m_bombPrefab;
    [SerializeField] int m_id;

    private GameObject m_bomb;

    private void Start()
    {
        var bomb = DataManager.Instance.GetBomb(m_id);
        m_bomb = Instantiate(m_bombPrefab);
        m_bomb.GetComponent<Bomb>().SetBomb(bomb);
    }

    public void OnGenerate()
    {
        m_bomb.GetComponent<Bomb>().Plant();
    }
}
