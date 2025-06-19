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
        switch (skill.TargetType)
        {
            case SkillTargetType.EnemySingle:
            case SkillTargetType.AllySingle:
                result = ApplySkillEffect(actor, target, skill);
                break;
            case SkillTargetType.EnemyAll:
                foreach (var enemy in allEnemies)
                    if (!enemy.IsDead) ApplySkillEffect(actor, enemy, skill);
                break;
            case SkillTargetType.AllyAll:
                foreach (var player in allPlayers)
                    if (!player.IsDead) ApplySkillEffect(actor, player, skill);
                break;
            case SkillTargetType.Self:
                ApplySkillEffect(actor, actor, skill);
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
        switch (skill.EffectType)
        {
            case SkillEffectType.Damage:
                if (actor.Team != target.Team && !target.IsDead)
                {
                    target.TakeDamage(skill.Power);
                    return true;
                }
                else
                {
                    Debug.LogWarning($"[Skill] 잘못된 대상에게 데미지 스킬 적용 시도!");
                    return false;
                }
            case SkillEffectType.Heal:
                if (target.IsDead)
                {
                    Debug.LogWarning($"[Skill] {target.UnitName}은(는) 사망 상태이므로 힐 불가!");
                    return false;
                }
                if ((skill.TargetType == SkillTargetType.AllySingle || skill.TargetType == SkillTargetType.AllyAll) &&
                    actor.Team == target.Team && actor != null && target != null)
                {
                    target.Heal(skill.Power);
                    return true;
                }
                else if (skill.TargetType == SkillTargetType.Self && actor == target)
                {
                    target.Heal(skill.Power);
                    return true;
                }
                else
                {
                    Debug.LogWarning($"[Skill] 잘못된 대상에게 회복 스킬 적용 시도!");
                    return false;
                }
            case SkillEffectType.Buff:
                if (actor.IsDead)
                {
                    Debug.LogWarning($"[Skill] {actor.UnitName}은(는) 사망 상태이므로 버프 불가!");
                    return false;
                }
                actor.ATK += skill.BuffValue;
                return true;
            default:
                return false;
        }
    }

    public async UniTask ExecuteEnemyAction(Unit attacker, Unit target)
    {
        Debug.Log($"[Combat] {attacker.UnitName} AI → {target.UnitName}");
        await UniTask.Delay(500);

        int damage = Mathf.Max(0, attacker.ATK - target.DEF);
        target.TakeDamage(damage);
    }
}