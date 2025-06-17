using Game.Input;

public enum PlayerActionType
{
    BasicAttack,
    Skill
}

public class PlayerAction
{
    public PlayerActionType Type;
    public Unit Actor;
    public Unit Target;
    public SkillData SkillData { get; set; }
}