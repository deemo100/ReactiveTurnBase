public enum ActionType
{
    BasicAttack,
    Skill
}

public struct PlayerAction
{
    public ActionType Type;   // BasicAttack or Skill
    public SkillData  Skill;  // Skill일 때만 값이 있음
}