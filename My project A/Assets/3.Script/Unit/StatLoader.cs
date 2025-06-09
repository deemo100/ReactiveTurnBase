using System.Collections.Generic;
using UnityEngine;

public class StatLoader : MonoBehaviour
{
    public TextAsset csvFile;
    public Dictionary<string, UnitStatData> statDict = new();

    private void Awake()
    {
        LoadStats();
    }

    private void LoadStats()
    {
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 연결되지 않았습니다.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var cols = lines[i].Split(',');

            var stat = new UnitStatData
            {
                id = int.Parse(cols[0]),
                name = cols[1],
                unitClass = cols[2],
                hp = int.Parse(cols[3]),
                attack = int.Parse(cols[4]),
                defense = int.Parse(cols[5]),
                groggy = int.Parse(cols[6])
            };

            statDict[stat.name] = stat;
        }

        Debug.Log($"[StatLoader] {statDict.Count}개 유닛 스탯 로딩 완료");
    }
    
    public UnitStatData GetStat(string name)
    {
        if (!statDict.ContainsKey(name))
        {
            Debug.LogWarning($"[StatLoader] Stat not found for: {name}");
            return null;
        }

        return statDict[name];
    }
}