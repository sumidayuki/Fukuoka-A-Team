using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class CSVReader
{
    public static List<Dictionary<string, string>> Read(string path)
    {
        var list = new List<Dictionary<string, string>>();
        if (!File.Exists(path)) return list;

        var lines = File.ReadAllLines(path);
        if (lines.Length < 2) return list;

        var headers = lines[0].Split(",");

        for(int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = lines[i].Split(',');
            var dict = new Dictionary<string, string>();

            for(int j = 0; j < headers.Length && j < values.Length; j++)
            {
                dict[headers[j]] = values[j];
            }

            list.Add(dict);
        }

        return list;
    }
}
