using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerRow
{
    public float moveSpeed;
    public int unlockedStageCount;
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/PlayerData")]
public class PlayerData : CSVDataBase
{
    public override string CSVFileName => "PlayerData.csv";

    public PlayerRow[] rows;
}
