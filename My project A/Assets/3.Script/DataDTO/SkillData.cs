namespace Game.Input
{
    public enum SkillTargetType
    {
        EnemySingle,    // 적 1명
        EnemyAll,       // 적 전체
        AllySingle,     // 아군 1명
        AllyAll,        // 아군 전체
        Self,           // 자기 자신
        // 필요시 추가
    }

    public enum SkillEffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        // 필요시 추가
    }

    public enum SkillCastType
    {
        Melee,      // 근접
        Ranged,     // 원거리/투사체/광역
        // 필요시 Magic, Support 등 세분화 가능
    }

    [System.Serializable]
    public class SkillData
    {
        public int Id;
        public string Name;
        public int Cost;
        public string IconName;
        public SkillTargetType TargetType;
        public SkillEffectType EffectType;
        public SkillCastType CastType;       // 추가! (근접/원거리)
        public int Power;                    // 공격력/회복력/버프수치 등
        public int BuffValue;                // 버프/디버프 값
        public int GroggyDamage; // ← 추가(스킬이 가하는 그로기 피해량)
        public float Duration;               // 지속시간(버프/디버프)
        public string Description;           // 설명(CSV에서 불러오기)
        // 필요하다면 AnimationClipName, 이펙트 프리팹 등 추가 가능

        // --- 생성자 및 유틸 함수도 필요하다면 추가 ---
        public SkillData() { }
        public SkillData(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}