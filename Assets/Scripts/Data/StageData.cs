using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageRow
{
    public int id;
    public string name;
    public int[] size;
    public int[] bombIds;
    public int[] bombCounts;
    public string bgm;
}

[CreateAssetMenu(fileName = "StageData", menuName = "GameData/StageData")]
public class StageData : CSVDataBase
{
    public override string CSVFileName => "StageData.csv";
    public StageRow[] rows;
}
