// Scripts/Unit/PlayerUnit.cs
using UnityEngine;

/// <summary>
/// 플레이어 유닛 공통 속성 및 Init 메서드
/// </summary>
public class PlayerUnit : MonoBehaviour
{
    [HideInInspector] public SkillData SkillData;

    public int MaxHP { get; private set; }
    public int CurrentHP { get; set; }
    public int AttackPower { get; private set; }
    public int Defense { get; private set; }
    public int MaxGroggy { get; private set; }
    public int GroggyGauge { get; set; }
    public int MaxCost { get; private set; }
    public int CurrentCost { get; set; }

    /// <summary>
    /// DataManager에서 읽어온 UnitStat 으로 각종 필드 초기화
    /// </summary>
    public void Init(UnitStat stat)
    {
        // 이름 설정 (Hierarchy에 표시)
        gameObject.name = stat.Name;

        MaxHP        = stat.MaxHP;
        CurrentHP    = stat.MaxHP;
        AttackPower  = stat.Attack;
        Defense      = stat.Defense;
        MaxGroggy    = stat.MaxGroggy;
        GroggyGauge  = stat.MaxGroggy;

        // 기본 코스트 (원하시면 CSV에 추가해서 세팅하세요)
        MaxCost    = 5;
        CurrentCost = MaxCost;
    }

    public bool IsDead => CurrentHP <= 0;
}