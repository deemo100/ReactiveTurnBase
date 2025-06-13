using UnityEngine;

public class PlayerUnit : MonoBehaviour, IInitializableUnit
{
    
    [Header("Stat Preview (디버그용)")]
    [SerializeField] private int _debugCurrentHP;
    [SerializeField] private int _debugMaxHP;
    [SerializeField] private int _debugAttack;
    [SerializeField] private int _debugDefense;
    
    [Header("CSV statId")]
    [SerializeField] int statId;
    [HideInInspector] public SkillData SkillData;

    private HealthBarFollower _healthBarUI;
    
    public int MaxHP     { get; private set; }
    public int CurrentHP { get; private set; }
    public int AttackPower{get;private set;}
    public int Defense{get;private set;}
    
    
    
    public bool IsDead=>CurrentHP<=0;

    
    void Update() {
        // Editor에서만 값 반영 (빌드시 무시)
#if UNITY_EDITOR
        _debugCurrentHP = CurrentHP;
        _debugMaxHP = MaxHP;
        _debugAttack = AttackPower;
        _debugDefense = Defense;
#endif
    }
    
    public void InitStat(int id)
    {
        statId = id;
        var stat = DataManager.Instance.UnitStatTable[id];
        MaxHP = stat.MaxHP; CurrentHP = stat.MaxHP;
        AttackPower=stat.Attack; Defense=stat.Defense;
        SkillData = DataManager.Instance.SkillTable[id];
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
    
    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }
}