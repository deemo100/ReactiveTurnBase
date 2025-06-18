using Game.Input;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int Id { get; protected set; }
    public string UnitName { get; protected set; }
    public string ClassName { get; protected set; }
    public int MaxHP { get; protected set; }
    public int HP { get; protected set; }
    public int ATK { get; protected set; }
    public int DEF { get; protected set; }
    public int MaxGroggy { get; protected set; }
    public int Groggy { get; protected set; }

    public bool IsDead => HP <= 0;
    public bool IsGroggy => Groggy <= 0;
    
    public HealthBar healthBar;
    public HealthBarFollower healthBarFollower;
    
    public virtual void Init(UnitStat stat)
    {
        Id = stat.Id;
        UnitName = stat.Name;
        ClassName = stat.ClassName;
        MaxHP = stat.MaxHP;
        HP = stat.MaxHP;
        ATK = stat.Attack;
        DEF = stat.Defense;
        MaxGroggy = stat.MaxGroggy;
        Groggy = stat.MaxGroggy;
    }
    public SkillData SkillData { get; protected set; }

    public virtual void TakeDamage(int amount)
    {
        HP = Mathf.Max(0, HP - amount);
        Debug.Log($"[Unit.TakeDamage] {UnitName}, HP: {HP}/{MaxHP}");
        if (healthBarFollower != null)
        {
            Debug.Log("[Unit] HP Bar SetHealth 호출됨");
            healthBarFollower.SetHealth(HP / (float)MaxHP);
        }
    }
    public virtual void Heal(int amount)
    {
        HP = Mathf.Min(MaxHP, HP + amount);
        if (healthBarFollower != null)
        {
            Debug.Log($"[Unit] Heal: {UnitName}, HP: {HP}/{MaxHP}");
            healthBarFollower.SetHealth(HP / (float)MaxHP);
        }
    }

    public virtual void TakeGroggy(int amount)
    {
        Groggy = Mathf.Max(0, Groggy - amount);
    }

    public virtual void RecoverGroggy(int amount)
    {
        Groggy = Mathf.Min(MaxGroggy, Groggy + amount);
    }
}