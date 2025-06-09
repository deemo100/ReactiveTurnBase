using System;
using System.Linq;                     // ← Skip, FirstOrDefault 등 LINQ 확장 메서드
using System.Collections.Generic;      // ← Dictionary, IEnumerable
using UnityEngine;

public interface IStatRepository
{
    UnitStatData GetStat(string unitName);
}

public class CsvStatRepository : MonoBehaviour, IStatRepository
{
    [SerializeField] private TextAsset csvFile;
    private Dictionary<string, UnitStatData> _stats;

    private void Awake()
    {
        if (csvFile == null)
        {
            Debug.LogError("[CsvStatRepository] CSV 파일이 할당되지 않았습니다.");
            return;
        }

        _stats = new Dictionary<string, UnitStatData>();

        // 첫 줄(header) 건너뛰고, 줄바꿈 기준으로 분리
        var lines = csvFile.text
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1);

        foreach (var line in lines)
        {
            // 빈 줄 혹은 공백만으로 된 줄은 건너뛰기
            if (string.IsNullOrWhiteSpace(line)) 
                continue;

            var cols = line.Split(',');

            // 최소 7개 열(id,name,class,hp,attack,defense,groggy)이 있어야 함
            if (cols.Length < 7)
            {
                Debug.LogWarning($"[CsvStatRepository] 잘못된 형식의 라인 건너뜀: {line}");
                continue;
            }

            // 데이터 파싱
            if (!int.TryParse(cols[0], out var id) ||
                !int.TryParse(cols[3], out var hp) ||
                !int.TryParse(cols[4], out var attack) ||
                !int.TryParse(cols[5], out var defense) ||
                !int.TryParse(cols[6], out var groggy))
            {
                Debug.LogWarning($"[CsvStatRepository] 숫자 파싱 실패: {line}");
                continue;
            }

            var data = new UnitStatData
            {
                id         = id,
                name       = cols[1].Trim(),
                unitClass  = cols[2].Trim(),
                hp         = hp,
                attack     = attack,
                defense    = defense,
                groggy     = groggy
            };

            _stats[data.name] = data;
        }

        Debug.Log($"[CsvStatRepository] {_stats.Count}개 유닛 스탯 로드 완료");
    }

    // IStatRepository 구현
    public UnitStatData GetStat(string unitName)
    {
        if (string.IsNullOrWhiteSpace(unitName)) 
            return null;

        unitName = unitName.Trim();
        return _stats.TryGetValue(unitName, out var stat) 
             ? stat 
             : null;
    }
}