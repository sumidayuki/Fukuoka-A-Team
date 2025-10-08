using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/PlayerData")]
public class PlayerData : CSVDataBase
{
    public override string CSVFileName => "PlayerData.csv";

    public float moveSpeed;
    public int unlockedStageCount;
}
