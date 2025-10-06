using UnityEngine;

public abstract class CSVDataBase : ScriptableObject
{
    // CSVファイル名を名SOで指定
    public abstract string CSVFileName { get; }
}
