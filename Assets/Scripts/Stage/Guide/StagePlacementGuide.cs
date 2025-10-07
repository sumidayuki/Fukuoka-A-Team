using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // エディタ上でも動作
public class StagePlacementGuide : MonoBehaviour
{
    [SerializeField] int stageID = 0;
    [SerializeField] StageData stageData;

    private Vector3 m_cellSize = new Vector3(2, 2, 2);
    private Color m_color = Color.green;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (stageData == null || stageData.rows == null) return;

        StageRow row = null;
        foreach (var r in stageData.rows)
        {
            if (r.id == stageID)
            {
                row = r;
                break;
            }
        }
        if (row == null) return;

        Gizmos.color = m_color;
        Vector3 start = transform.position;

        for (int x = 0; x < row.size[0]; x++)
        {
            for (int y = 0; y < row.size[1]; y++)
            {
                for (int z = 0; z < row.size[2]; z++)
                {
                    Vector3 pos = start + Vector3.Scale(new Vector3(x, y, z), m_cellSize);
                    Gizmos.DrawWireCube(pos - new Vector3(0.5f, 0.5f, 0.5f) + m_cellSize * 0.5f, m_cellSize);
                }
            }
        }
    }
#endif
}
