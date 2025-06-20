using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Input;
using System.Collections.Generic;

public class SimpleCombatExecutor : MonoBehaviour
{
    bool result = false;
    
    public async UniTask ExecuteBasicAttack(Unit attacker, Unit target)
    {
        Debug.Log($"[Combat] {attacker.UnitName} BasicAttack → {target.UnitName}");
        await UniTask.Delay(500);

        // 데미지 공식 예시 (공격력 - 방어력, 0 이하 방지)
        int damage = Mathf.Max(0, attacker.ATK - target.DEF);
        target.TakeDamage(damage);
    }

    public async UniTask<bool> ExecuteSkill(
        PlayerUnit actor,
        Unit target,
        SkillData skill,
        List<PlayerUnit> allPlayers,
        List<EnemyUnit> allEnemies)
    {
        bool result = false;
        switch (skill.TargetType)
        {
            case SkillTargetType.EnemySingle:
            case SkillTargetType.AllySingle:
                result = ApplySkillEffect(actor, target, skill);
                break;
            case SkillTargetType.EnemyAll:
                foreach (var enemy in allEnemies)
                {
                    Debug.Log($"[디버그] 대상: {enemy.UnitName}, Dead: {enemy.IsDead}");
                    if (!enemy.IsDead)
                    {
                        bool eff = ApplySkillEffect(actor, enemy, skill);
                        Debug.Log($"[디버그] {enemy.UnitName} 효과 적용됨: {eff}");
                        if (eff) result = true;
                    }
                }
                break;
            case SkillTargetType.AllyAll:
                foreach (var player in allPlayers)
                {
                    if (!player.IsDead)
                    {
                        if (ApplySkillEffect(actor, player, skill)) result = true;
                    }
                }
                break;
            
            case SkillTargetType.Self:
                result = ApplySkillEffect(actor, actor, skill);
                break;
        }
        if (!result)
        {
            Debug.LogWarning("[Skill] 실패! 턴/코스트를 소모하지 않습니다.");
            return false; // 실패!
        }
        await UniTask.Delay(300);
        return true; // 성공!
    }

    private bool ApplySkillEffect(PlayerUnit actor, Unit target, SkillData skill)
    {
        if (target == null) return false;      // ⭐ null 체크 선행!
        if (target.IsDead) return false;       // 죽은 대상은 패스
        
        switch (skill.EffectType)
        {
            case SkillEffectType.Damage:
                Debug.Log($"[디버그] {actor.UnitName}→{target.UnitName}, 팀: {actor.Team} vs {target.Team}");
                if (actor.Team != target.Team)
                {
                    Debug.Log($"[디버그] {target.UnitName}에게 {skill.Power} 데미지!");
                    target.TakeDamage(skill.Power);
                    return true;
                }
                break;
            case SkillEffectType.Heal:
                if (actor.Team == target.Team)
                {
                    target.Heal(skill.Power);
                    return true;
                }
                break;
            case SkillEffectType.Buff:
                // ... 버프 로직
                return true;
        }
        return false;
    }

    public async UniTask ExecuteEnemyAction(Unit attacker, Unit target)
    {
        Debug.Log($"[Combat] {attacker.UnitName} AI → {target.UnitName}");
        await UniTask.Delay(500);

        int damage = Mathf.Max(0, attacker.ATK - target.DEF);
        target.TakeDamage(damage);
    }
}