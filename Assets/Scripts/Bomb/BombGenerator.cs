using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombGenerator : MonoBehaviour
{
    [SerializeField] GameObject m_bombPrefab;
    [SerializeField] int m_id;

    public void OnGenerate()
    {
        var bomb = DataManager.Instance.GetBomb(m_id);
        
        for(int x = 0; x < bomb.x.Length; x++)
        {
            for (int i = 0; i < bomb.x[x]; i++)
            {
                Vector3 dir = x == 0 ? Vector3.left : Vector3.right;
                Instantiate(m_bombPrefab, this.transform.position + dir * (i + 1), Quaternion.identity);
            }
        }
        for (int y = 0; y < bomb.y.Length; y++)
        {
            for (int i = 0; i < bomb.y[y]; i++)
            {
                Vector3 dir = y == 0 ? Vector3.up : Vector3.down;
                Instantiate(m_bombPrefab, this.transform.position + dir * i, Quaternion.identity);
            }
        }
        for (int z = 0; z < bomb.z.Length; z++)
        {
            for (int i = 0; i < bomb.z[z]; i++)
            {
                Vector3 dir = z == 0 ? Vector3.forward : Vector3.back;
                Instantiate(m_bombPrefab, this.transform.position + dir * (i + 1), Quaternion.identity);
            }
        }
    }
}
