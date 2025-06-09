using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CsvSkillRepository : MonoBehaviour, ISkillRepository
{
    [SerializeField] private TextAsset csvFile;  

    private Dictionary<int, SkillData> _skills;

    private void Awake()
    {
        if (csvFile == null)
        {
            Debug.LogError("[CsvSkillRepository] Skills.csv 할당 필요");
            return;
        }

        _skills = new Dictionary<int, SkillData>();

        var lines = csvFile.text
            .Split(new[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1);

        foreach (var line in lines)
        {
            var cols = line.Split(',');
            if (cols.Length < 5) continue;

            if (!int.TryParse(cols[0], out var id) ||
                !int.TryParse(cols[2], out var cost))
            {
                Debug.LogWarning($"[CsvSkillRepo] 파싱 실패: {line}");
                continue;
            }

            var data = new SkillData {
                id         = id,
                name       = cols[1].Trim(),
                cost       = cost,
                icon       = Resources.Load<Sprite>($"SkillIcons/{cols[3].Trim()}"),
                targetType = Enum.Parse<TargetType>(cols[4].Trim())
            };

            _skills[id] = data;
        }

        Debug.Log($"[CsvSkillRepository] {_skills.Count}개 스킬 로드 완료");
    }

    public IReadOnlyList<SkillData> GetAllSkills() =>
        _skills.Values.ToList();

    public SkillData GetSkillById(int id) =>
        _skills.TryGetValue(id, out var s) ? s : null;
}