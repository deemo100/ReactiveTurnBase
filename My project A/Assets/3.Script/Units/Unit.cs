using Game.Input;
using UnityEngine;

public enum TeamType
{
    Player,
    Enemy
}
public class Unit : MonoBehaviour
{
    public TeamType Team { get; set; }

    public int Id { get; protected set; }
    public string UnitName { get; protected set; }
    public string ClassName { get; protected set; }
    public int MaxHP { get; protected set; }
    public int HP { get; protected set; }
    public int ATK { get; set; }
    public int DEF { get; protected set; }
    public int MaxGroggy { get; protected set; }
    public int Groggy { get; protected set; }

    public bool IsDead => HP <= 0;
    public bool IsGroggy => Groggy <= 0;

    
    public HealthBar healthBar;
    public HealthBarFollower healthBarFollower;
    public SkillData SkillData { get; protected set; }

    public virtual void Init(UnitStat stat, TeamType team)
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
        Team = team;
    }

    public virtual void TakeDamage(int amount)
    {
        Debug.Log($"[Unit] TakeDamage 호출됨, {UnitName}, 타입: {this.GetType()}");
        HP = Mathf.Max(0, HP - amount);
        Debug.Log($"[Unit.TakeDamage] {UnitName}, HP: {HP}/{MaxHP}");

        if (healthBarFollower != null)
            healthBarFollower.SetHealth(HP / (float)MaxHP);

        // ★ 반드시 CheckVictory를 공통으로 호출
        if (HP <= 0)
        {
            DefaultTurnManager.Instance?.CheckVictory();
        }

        // 기본 애니메이션 트리거 처리 (자식에서 오버라이드 가능)
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            if (HP > 0)
                animator.SetTrigger("3_Damaged");
            else
                animator.SetTrigger("4_Death");
        }
    }

    public virtual void PlayAttackAnim()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("2_Attack");
        }
    }

    public virtual void PlaySkillAnim()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("7_Skill"); // 스킬
    }
    
    public virtual void Heal(int amount)
    {
        HP = Mathf.Min(MaxHP, HP + amount);
        if (healthBarFollower != null)
        {
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
