using UnityEngine;

public class EnemyUnit : MonoBehaviour, IInitializableUnit
{
    
    [Header("Stat Preview (디버그용)")]
    [SerializeField] private int _debugCurrentHP;
    [SerializeField] private int _debugMaxHP;
    [SerializeField] private int _debugAttack;
    [SerializeField] private int _debugDefense;
    
    [Header("CSV statId")]
    [SerializeField] private int statId;

    [HideInInspector] public SkillData SkillData;

    private HealthBarFollower _healthBarUI;
    
    public int MaxHP     { get; private set; }
    public int CurrentHP { get; private set; }
    public int Attack    { get; private set; }
    public int Defense   { get; private set; }

    public bool IsDead => CurrentHP <= 0;

    void Update()
    {
#if UNITY_EDITOR
        _debugCurrentHP = CurrentHP;
        _debugMaxHP = MaxHP;
        _debugAttack = Attack;
        _debugDefense = Defense;
#endif
    }
    
    public void InitStat(int id)
    {
        statId = id;
        var stat = DataManager.Instance.UnitStatTable[id];
        MaxHP     = stat.MaxHP;
        CurrentHP = stat.MaxHP;
        Attack    = stat.Attack;
        Defense   = stat.Defense;

        SkillData = DataManager.Instance.SkillTable[id];
        // ★ SPUM_Prefabs 관련 코드 전부 삭제
        if (_healthBarUI != null)
            _healthBarUI.SetHealth(CurrentHP / (float)MaxHP);
    }

    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(CurrentHP - amount, 0);
        if (_healthBarUI != null)
            _healthBarUI.SetHealth(CurrentHP / (float)MaxHP);
    }
    public void BindHealthBar(HealthBarFollower hb)
    {
        _healthBarUI = hb;
        hb.SetHealth(CurrentHP / (float)MaxHP);
    }
    
}