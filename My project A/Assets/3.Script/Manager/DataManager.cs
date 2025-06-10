// Scripts/Data/DataManager.cs
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CSV 파일로부터 SkillData 와 UnitStat 정보를 읽어들여 Dictionary 형태로 보관하는 싱글톤 매니저
/// </summary>
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public Dictionary<int, SkillData> SkillTable    { get; private set; }
    public Dictionary<int, UnitStat>  UnitStatTable { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SkillTable    = new Dictionary<int, SkillData>();
            UnitStatTable = new Dictionary<int, UnitStat>();
            LoadSkills();
            LoadUnitStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSkills()
    {
        var ta = Resources.Load<TextAsset>("Skills");
        if (ta == null)
        {
            Debug.LogError("Failed to load Skills.csv from Resources folder.");
            return;
        }

        using var reader = new StringReader(ta.text);
        bool isHeader = true;
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            if (isHeader)
            {
                isHeader = false;
                continue;
            }
            var cols = line.Split(',');
            if (cols.Length < 5) continue;

            var sd = new SkillData
            {
                Id         = int.Parse(cols[0].Trim()),
                Name       = cols[1].Trim(),
                Cost       = int.Parse(cols[2].Trim()),
                IconName   = cols[3].Trim(),
                TargetType = Enum.Parse<TargetType>(cols[4].Trim(), ignoreCase: true)
            };
            SkillTable[sd.Id] = sd;
        }
    }

    private void LoadUnitStats()
    {
        var ta = Resources.Load<TextAsset>("UnitStats");
        if (ta == null)
        {
            Debug.LogError("Failed to load UnitStats.csv from Resources folder.");
            return;
        }

        using var reader = new StringReader(ta.text);
        bool isHeader = true;
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            if (isHeader)
            {
                isHeader = false;
                continue;
            }
            var cols = line.Split(',');
            if (cols.Length < 7) continue;

            var us = new UnitStat
            {
                Id        = int.Parse(cols[0].Trim()),
                Name      = cols[1].Trim(),
                ClassName = cols[2].Trim(),
                MaxHP     = int.Parse(cols[3].Trim()),
                Attack    = int.Parse(cols[4].Trim()),
                Defense   = int.Parse(cols[5].Trim()),
                MaxGroggy = int.Parse(cols[6].Trim())
            };
            UnitStatTable[us.Id] = us;
        }
    }
}
