using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private AudioClip tickSE;
    [SerializeField] private AudioClip explodeSE;

    private bool m_isExplosed;

    private void OnDisable()
    {
       if(StageManager.Instance != null)
        {
            StageManager.Instance.OnBombExploded(this);
        }
    }

    public void Explode()
    {
        if (SoundManager.Instance != null && explodeSE != null)
            SoundManager.Instance.PlaySE(explodeSE);
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
            GameObject obj = Instantiate(m_explosivePrefab, m_explosiveTransform.position + Vector3.back * (i + 1), Quaternion.identity);
            obj.transform.SetParent(m_explosiveTransform);
        }
    }

    private void Update()
    {
        if(this.gameObject.activeSelf)
        {
            m_myBomb.time -= Time.deltaTime;
                
            if(m_myBomb.time < 0 && !m_isExplosed)
            {
                Explode();
                m_isExplosed = true;
            }
        }
    }

    public void Plant(Transform transform)
    {
        gameObject.transform.position = transform.position;

        Vector3 euler = transform.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;
        transform.rotation = Quaternion.Euler(euler);

        m_bombPrefab.gameObject.SetActive(true);
        m_explosiveTransform.gameObject.SetActive(false);
    }

    public void SetBomb(BombRow data)
    {
        m_myBomb.left = data.x[0];
        m_myBomb.right = data.x[1];
        m_myBomb.up = data.y[0];
        m_myBomb.down = 0;
        m_myBomb.forward = data.z[0];
        m_myBomb.back = data.z[1];
        m_myBomb.time = 3.0f;

        m_isExplosed = false;

        GenerateExplodeRange();
    }
}
