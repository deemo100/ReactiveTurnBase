using UnityEngine;

public class PlayerUnit : Unit
{
    // 플레이어 전용 행동/상태만 여기!
    // 예시: 턴 내 행동 상태
    public bool HasAttackedThisTurn { get; private set; }
    public bool HasUsedSkillThisTurn { get; private set; }

    public void ResetTurnState()
    {
        HasAttackedThisTurn = false;
        HasUsedSkillThisTurn = false;
    }

    public void MarkAttack() => HasAttackedThisTurn = true;
    public void MarkSkill()  => HasUsedSkillThisTurn = true;

    public bool CanAttack => !HasAttackedThisTurn && !IsDead && !IsGroggy;
    public bool CanUseSkill(int cost, int currentCost) =>
        !HasUsedSkillThisTurn && (currentCost >= cost) && !IsDead && !IsGroggy;
}