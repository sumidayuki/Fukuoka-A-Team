using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BombRow
{
    public int id;
    public string name;
    public int[] x;
    public int[] y;
    public int[] z;
    public float time;
}

[CreateAssetMenu(fileName = "BombData", menuName = "GameData/BombData")]
public class BombData : CSVDataBase
{
    public override string CSVFileName => "BombData.csv";
    public BombRow[] rows;
}