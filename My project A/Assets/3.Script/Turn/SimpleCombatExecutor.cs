using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Input;
using System.Collections.Generic;

public class SimpleCombatExecutor : MonoBehaviour
{
    public async UniTask ExecuteBasicAttack(Unit attacker, Unit target)
    {
        Debug.Log($"[Combat] {attacker.UnitName} BasicAttack → {target.UnitName}");
        await UniTask.Delay(500);

        // 데미지 공식 예시 (공격력 - 방어력, 0 이하 방지)
        int damage = Mathf.Max(0, attacker.ATK - target.DEF);
        target.TakeDamage(damage);
    }

    public async UniTask ExecuteSkill(
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
                ApplySkillEffect(actor, target, skill);
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
        await UniTask.Delay(300);
    }

    private void ApplySkillEffect(PlayerUnit actor, Unit target, SkillData skill)
    {
        switch (skill.EffectType)
        {
            case SkillEffectType.Damage:
                Debug.Log($"[Skill] {actor.UnitName}이(가) {skill.Name} (위력:{skill.Power})로 {target.UnitName}에게 {skill.Power} 데미지!");
                target.TakeDamage(skill.Power);
                break;
            case SkillEffectType.Heal:
                Debug.Log($"[Skill] {actor.UnitName}이(가) {skill.Name} (회복:{skill.Power})로 {target.UnitName}을 {skill.Power}만큼 회복!");
                target.Heal(skill.Power);
                break;
            case SkillEffectType.Buff:
                Debug.Log($"[Skill] {actor.UnitName}이(가) {skill.Name} (버프:{skill.BuffValue})로 자신의 공격력을 {skill.BuffValue} 증가!");
                actor.ATK += skill.BuffValue;
                break;
            default:
                Debug.Log($"[Skill] {actor.UnitName}이(가) {skill.Name} 사용! (타입:{skill.EffectType})");
                break;
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