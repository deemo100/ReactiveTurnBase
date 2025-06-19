namespace Game.Input
{
    public enum SkillTargetType
    {
        EnemySingle,    // 적 1명
        EnemyAll,       // 적 전체
        AllySingle,     // 아군 1명
        AllyAll,        // 아군 전체
        Self,           // 자기 자신
    }

    public enum SkillEffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        // 필요시 추가
    }

    public class SkillData
    {
        public int Id;
        public string Name;
        public int Cost;
        public string IconName;
        public SkillTargetType TargetType;
        public SkillEffectType EffectType;
        public int Power;      // 공격력/회복력 등
        public int BuffValue;  // 버프/디버프용
        public string Description;  // <-- CSV의 Description 열에 들어간 값
    }
}