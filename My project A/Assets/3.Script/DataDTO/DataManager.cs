using System;
using System.IO;
using System.Collections.Generic;
using Game.Input;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public Dictionary<int, SkillData> SkillTable { get; private set; }
    public Dictionary<int, UnitStat> UnitStatTable { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SkillTable = new Dictionary<int, SkillData>();
            UnitStatTable = new Dictionary<int, UnitStat>();
            LoadSkills();
            LoadUnitStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ Skill id로 SkillData 반환
    public SkillData GetSkillById(int id)
    {
        if (SkillTable.TryGetValue(id, out var skill))
            return skill;
        Debug.LogWarning($"[DataManager] Skill id={id} 없음!");
        return null;
    }

    private void LoadSkills()
    {
        var ta = Resources.Load<TextAsset>("Skills");
        using var reader = new StringReader(ta.text);
        bool header = true;
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            if (header) { header = false; continue; }
            var cols = line.Split(',');
            var sd = new SkillData {
                Id          = int.Parse(cols[0].Trim()),
                Name        = cols[1].Trim(),
                Cost        = int.Parse(cols[2].Trim()),
                IconName    = cols[3].Trim(),
                TargetType  = Enum.Parse<SkillTargetType>(cols[4].Trim(), true),
                EffectType  = Enum.Parse<SkillEffectType>(cols[5].Trim(), true),
                Power       = int.Parse(cols[6].Trim()),
                BuffValue   = int.Parse(cols[7].Trim()),
                Description = cols[8].Trim()         // <-- 여기!
            };
            SkillTable[sd.Id] = sd;
        }
    }

    private void LoadUnitStats()
    {
        var ta = Resources.Load<TextAsset>("UnitStats");
        if (ta == null)
        {
            Debug.LogError("Failed to load UnitStats.csv");
            return;
        }

        using var reader = new StringReader(ta.text);
        bool header = true;
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            if (header)
            {
                header = false;
                continue;
            }

            var cols = line.Split(',');
            if (cols.Length < 8) continue; // SkillId 포함 8컬럼 이상

            var us = new UnitStat
            {
                Id = int.Parse(cols[0].Trim()),
                Name = cols[1].Trim(),
                ClassName = cols[2].Trim(),
                MaxHP = int.Parse(cols[3].Trim()),
                Attack = int.Parse(cols[4].Trim()),
                Defense = int.Parse(cols[5].Trim()),
                MaxGroggy = int.Parse(cols[6].Trim()),
                SkillId = int.Parse(cols[7].Trim()) // ← SkillId
            };
            UnitStatTable[us.Id] = us;
        }
    }
}