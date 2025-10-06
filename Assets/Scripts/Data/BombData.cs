using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BombRow
{
    public int id;
    public string name;
    public Vector2 x;
    public Vector2 y;
    public Vector2 z;
}

[CreateAssetMenu(fileName = "BombData", menuName = "GameData/BombData")]
public class BombData : CSVDataBase
{
    public override string CSVFileName => "BombData.csv";
    public BombRow[] rows;
}