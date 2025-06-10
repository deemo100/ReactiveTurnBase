// Scripts/Turn/PlayerCommand.cs
using UnityEngine;

/// <summary>
/// 플레이어가 선택한 행동(공격 or 스킬)과 그 대상 정보를 담는 DTO
/// </summary>
public class PlayerCommand
{
    /// <summary>스킬 사용 여부</summary>
    public bool       IsSkill;

    /// <summary>사용할 스킬 (IsSkill == true일 때)</summary>
    public SkillData  Skill;

    /// <summary>공격/스킬 대상 (EnemyUnit 또는 PlayerUnit)</summary>
    public Component  Target;
}