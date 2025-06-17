namespace Game.Input
{
    public enum TargetType
    {
        EnemyOnly,
        AllyOnly,
        All
    }

    public class SkillData
    {
        public int Id;
        public string Name;
        public int Cost;
        public string IconName;
        public TargetType TargetType;
    }
    
}