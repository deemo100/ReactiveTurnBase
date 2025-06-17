// Assets/Scripts/Turn/DTO/PlayerCommand.cs

using Game.Input;
using UnityEngine;

public class PlayerCommand
{
    public bool      IsSkill;
    public SkillData Skill;      // 스킬일 때만 채워지고, 아니면 null
    public Component Target;     // EnemyUnit 또는 PlayerUnit
}