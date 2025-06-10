// Scripts/Unit/EnemyUnit.cs
using UnityEngine;

/// <summary>
/// 적 유닛 공통 속성 및 Init 메서드
/// </summary>
public class EnemyUnit : MonoBehaviour
{
    [HideInInspector] public SkillData SkillData;

    public int MaxHP { get; private set; }
    public int CurrentHP { get; set; }
    public int Attack  { get; private set; }
    public int Defense { get; private set; }
    public int MaxGroggy { get; private set; }
    public int GroggyGauge { get; set; }
    public bool IsStunned { get; set; }

    /// <summary>
    /// DataManager에서 읽어온 UnitStat 으로 각종 필드 초기화
    /// </summary>
    public void Init(UnitStat stat)
    {
        gameObject.name = stat.Name;

        MaxHP       = stat.MaxHP;
        CurrentHP   = stat.MaxHP;
        Attack      = stat.Attack;
        Defense     = stat.Defense;
        MaxGroggy   = stat.MaxGroggy;
        GroggyGauge = stat.MaxGroggy;

        IsStunned = false;
    }

    public bool IsDead => CurrentHP <= 0;
}