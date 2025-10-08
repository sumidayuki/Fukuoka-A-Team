using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    struct MyBomb
    {
        public int left;
        public int right;
        public int up;
        public int down;
        public int forward;
        public int back;
        public float time;
    }

    MyBomb m_myBomb;

    [SerializeField] GameObject m_bombPrefab;
    [SerializeField] Transform m_explosiveTransform;
    [SerializeField] GameObject m_explosivePrefab;

    public void Explode()
    {
        m_bombPrefab.SetActive(false);
        m_explosiveTransform.gameObject.SetActive(true);
        Destroy(this.gameObject, 0.5f);
    }

    private void GenerateExplodeRange()
    {
        for (int i = 0; i < m_myBomb.left; i++)
        {
            GameObject obj = Instantiate(m_explosivePrefab, m_explosiveTransform.position + Vector3.left * (i + 1), Quaternion.identity);
            obj.transform.SetParent(m_explosiveTransform);
        }
        for (int i = 0; i < m_myBomb.right; i++)
        {
            GameObject obj = Instantiate(m_explosivePrefab, m_explosiveTransform.position + Vector3.right * (i + 1), Quaternion.identity);
            obj.transform.SetParent(m_explosiveTransform);
        }
        for (int i = 0; i < m_myBomb.up; i++)
        {
            GameObject obj = Instantiate(m_explosivePrefab, m_explosiveTransform.position + Vector3.up * (i), Quaternion.identity);
            obj.transform.SetParent(m_explosiveTransform);
        }
        for (int i = 0; i < m_myBomb.down; i++)
        {   
            GameObject obj = Instantiate(m_explosivePrefab, m_explosiveTransform.position + Vector3.down * (i + 1), Quaternion.identity);
            obj.transform.SetParent(m_explosiveTransform);
        }
        for (int i = 0; i < m_myBomb.forward; i++)
        {
            GameObject obj = Instantiate(m_explosivePrefab, m_explosiveTransform.position + Vector3.forward * (i + 1), Quaternion.identity);
            obj.transform.SetParent(m_explosiveTransform);
        }
        for (int i = 0; i < m_myBomb.back; i++)
        {
            GameObject obj = Instantiate(m_bombPrefab, m_explosiveTransform.position + Vector3.back * (i + 1), Quaternion.identity);
            obj.transform.SetParent(m_explosiveTransform);
        }
    }

    private void Update()
    {
        if(m_bombPrefab.activeSelf)
        {
            m_myBomb.time -= Time.deltaTime;

            if(m_myBomb.time < 0)
            {
                Explode();
            }
        }
    }

    public void Plant()
    {
        gameObject.SetActive(true);
        m_bombPrefab.gameObject.SetActive(true);
        m_explosiveTransform.gameObject.SetActive(false);
    }

    public void SetBomb(BombRow data)
    {
        m_myBomb.left = data.x[0];
        m_myBomb.right = data.x[1];
        m_myBomb.up = data.y[0];
        m_myBomb.down = data.y[1];
        m_myBomb.forward = data.z[0];
        m_myBomb.back = data.z[1];
        m_myBomb.time = 3.0f;

        GenerateExplodeRange();

        this.gameObject.SetActive(false);
    }
}
