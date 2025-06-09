using UnityEngine;
using System;
public class Unit : MonoBehaviour
{
    public string unitName;
    public string unitClass;
    public int level = 1;

    public int maxHP, currentHP;
    public int attackPower, defense;
    public int maxGroggy, currentGroggy;

    private int stunTurns = 0;

    public bool IsEnemy => unitClass == "None";
    public bool IsPlayer => !IsEnemy;
    public bool IsStunned => stunTurns > 0 || (IsEnemy && currentGroggy <= 0);

    public static event Action<Unit> OnUnitClicked;
    
    
    public void ApplyStat(UnitStatData stat)
    {
        unitClass = stat.unitClass;
        maxHP = currentHP = stat.hp;
        attackPower = stat.attack;
        defense = stat.defense;
        maxGroggy = currentGroggy = stat.groggy;
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        Debug.Log($"{unitName} ▶ 피해 {amount} → 남은 HP {currentHP}");
    }

    public void Attack(Unit target)
    {
        Debug.Log($"{unitName} ▶ {target.unitName}에게 공격");
        target.TakeDamage(attackPower);
    }

    public void ApplyStunTurns(int turns)
    {
        stunTurns = Mathf.Max(stunTurns, turns);
        Debug.Log($"{unitName} ▶ {turns}턴간 스턴!");
    }

    public void DecreaseStunTurn()
    {
        if (stunTurns > 0)
        {
            stunTurns--;
            if (stunTurns == 0)
                Debug.Log($"{unitName} ▶ 스턴 해제됨");
        }
    }

    public void TakeGroggyDamage(int amount)
    {
        if (IsPlayer) return;
        currentGroggy = Mathf.Max(0, currentGroggy - amount);
        Debug.Log($"{unitName} ▶ 그로기 감소 {amount} → {currentGroggy}");
        if (currentGroggy <= 0)
            Debug.Log($"{unitName} ▶ 그로기 → 스턴 상태 진입");
    }
    
    private void OnMouseDown()
    {
        // 죽었거나 비적용 유닛 클릭 시 무시
        if (currentHP <= 0) return;
        OnUnitClicked?.Invoke(this);
    }
    
    
}