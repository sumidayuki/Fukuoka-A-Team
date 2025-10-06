#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

public static class CSVToSOConverter
{
    [MenuItem("Tools/CSV/Convert ALL CSVs")]
    public static void ConvertALLCSVs()
    {
        string csvDir = "Assets/CSVs/";
        string soDir = "Assets/ScriptableObjects/";
        if (!Directory.Exists(csvDir)) Directory.CreateDirectory(csvDir);
        if (!Directory.Exists(soDir)) Directory.CreateDirectory(soDir);

        var soTypes = typeof(CSVDataBase).Assembly.GetTypes();
        foreach (var type in soTypes)
        {
            if (type.IsAbstract || !typeof(CSVDataBase).IsAssignableFrom(type)) continue;

            var temp = ScriptableObject.CreateInstance(type) as CSVDataBase;
            string csvPath = Path.Combine(csvDir, temp.CSVFileName);
            ConvertOne(type, csvPath, soDir);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("ëSCSV Å® SOïœä∑äÆóπ");
    }

    private static void ConvertOne(Type soType, string csvPath, string soDir)
    {
        var csv = CSVReader.Read(csvPath);
        if (csv.Count == 0) return;

        var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(Path.Combine(soDir, soType.Name + ".asset")) ?? ScriptableObject.CreateInstance(soType);

        var rowField = soType.GetField("rows");
        var rowType = rowField.FieldType.GetElementType();
        Array rowArray = Array.CreateInstance(rowType, csv.Count);

        for(int i = 0; i < csv.Count; i++)
        {
            var dict = csv[i];
            var row = Activator.CreateInstance(rowType);
            foreach (var field in rowType.GetFields())
            {
                if(dict.TryGetValue(field.Name, out string value))
                {
                    try { field.SetValue(row, Convert.ChangeType(value, field.FieldType)); }
                    catch { Debug.LogWarning($"CSVïœä∑é∏îs"); }
                }
            }
            rowArray.SetValue(row, i);
        }

        rowField.SetValue(asset, rowArray);
    }
}

#endif
