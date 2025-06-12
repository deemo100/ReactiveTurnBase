// 3. Scripts/Data/DataManager.cs
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-1000)]
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public Dictionary<int, SkillData> SkillTable    { get; private set; }
    public Dictionary<int, UnitStat>  UnitStatTable { get; private set; }

    void Awake()
    {
        Debug.Log("DataManager Awake 실행됨");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SkillTable    = new Dictionary<int, SkillData>();
            UnitStatTable = new Dictionary<int, UnitStat>();
            LoadSkills();
            LoadUnitStats();
            Debug.Log("DataManager 싱글턴 할당됨");
        }
        else
        {
            Debug.LogWarning("중복 DataManager 파괴");
            Destroy(gameObject);
        }
    }

    private void LoadSkills()
    {
        var ta = Resources.Load<TextAsset>("Skills");
        if (ta == null) { Debug.LogError("Failed to load Skills.csv"); return; }
        using var reader = new StringReader(ta.text);
        bool header = true;
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            if (header) { header = false; continue; }
            var cols = line.Split(','); if (cols.Length < 5) continue;
            var sd = new SkillData {
                Id         = int.Parse(cols[0].Trim()),
                Name       = cols[1].Trim(),
                Cost       = int.Parse(cols[2].Trim()),
                IconName   = cols[3].Trim(),
                TargetType = Enum.Parse<TargetType>(cols[4].Trim(), true)
            };
            SkillTable[sd.Id] = sd;
        }
    }

    private void LoadUnitStats()
    {
        var ta = Resources.Load<TextAsset>("UnitStats");
        if (ta == null) { Debug.LogError("Failed to load UnitStats.csv"); return; }
        using var reader = new StringReader(ta.text);
        bool header = true;
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            if (header) { header = false; continue; }
            var cols = line.Split(',');
            if (cols.Length < 7) continue;

            // 여기가 체크 구간!
            var idStr = cols[0].Trim();
            int idVal;
            if (!int.TryParse(idStr, out idVal)) {
                Debug.LogError($"id 파싱 실패: 원본[{cols[0]}] / Trim[{idStr}] / Line={line}");
                continue;
            }
            if (UnitStatTable.ContainsKey(idVal)) {
                Debug.LogError($"중복 id 발견: {idVal}");
            }
            Debug.Log($"[UnitStat] Add id={idVal}");

            var us = new UnitStat {
                Id        = idVal,
                Name      = cols[1].Trim(),
                ClassName = cols[2].Trim(),
                MaxHP     = int.Parse(cols[3].Trim()),
                Attack    = int.Parse(cols[4].Trim()),
                Defense   = int.Parse(cols[5].Trim()),
                MaxGroggy = int.Parse(cols[6].Trim())
            };

            UnitStatTable[idVal] = us;
        }
    }
}
