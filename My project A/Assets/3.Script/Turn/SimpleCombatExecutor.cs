using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Input;
using System.Collections.Generic;

public class SimpleCombatExecutor : MonoBehaviour
{
    bool result = false;

    public async UniTask ExecuteBasicAttack(Unit attacker, Unit target)
    {
        if (target == null || target.IsDead) return;
        Debug.Log($"[Combat] {attacker.UnitName} BasicAttack → {target.UnitName}");

        // 원거리면 이동 없음 (플래그가 있다면)
        if (attacker.AttackType == AttackRangeType.Ranged)
        {
            attacker.LookAt(target.transform.position);
            attacker.PlayAttackAnim();

            float animTime = attacker.GetCurrentAttackAnimLength();
            await UniTask.Delay((int)(animTime * 1000));
            await UniTask.Delay(500);

            int damage = Mathf.Max(0, attacker.ATK - target.DEF);
            target.TakeDamage(damage);
            return;
        }

        // 근접은 이동 → 공격 → 복귀
        float moveSpeed = attacker.MoveSpeed;
        float approachDistance = 2f;
        Vector3 attackPos = target.transform.position + new Vector3(
            attacker.Team == TeamType.Player ? -approachDistance : approachDistance,
            0, 0
        );

        attacker.LookAt(attackPos);
        await attacker.MoveTo(attackPos, moveSpeed);

        attacker.PlayAttackAnim();

        float animTime2 = attacker.GetCurrentAttackAnimLength();
        await UniTask.Delay((int)(animTime2 * 1000));
        await UniTask.Delay(500);

        int damage2 = Mathf.Max(0, attacker.ATK - target.DEF);
        target.TakeDamage(damage2);

        attacker.LookAt(attacker.SpawnPosition);
        await attacker.MoveTo(attacker.SpawnPosition, moveSpeed);
        attacker.ResetRotation();
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
        if (target == null) return false;
        if (target.IsDead) return false;

        switch (skill.EffectType)
        {
            case SkillEffectType.Damage:
                Debug.Log($"[디버그] {actor.UnitName}→{target.UnitName}, 팀: {actor.Team} vs {target.Team}");
                if (actor.Team != target.Team)
                {
                    Debug.Log($"[디버그] {target.UnitName}에게 {skill.Power} 데미지!");
                    // ★ 스킬 데미지 애니메이션 실행!
                    actor.PlaySkillAnim();
                    target.TakeDamage(skill.Power);
                    return true;
                }
                break;
            case SkillEffectType.Heal:
                if (actor.Team == target.Team)
                {
                    if (target.HP < target.MaxHP)
                    {
                        // 필요하다면 여기서도 actor.PlaySkillAnim() 가능
                        target.Heal(skill.Power);
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"[Skill] {target.UnitName}은(는) 이미 체력이 가득 참! 힐 무시됨.");
                    }
                }
                break;
            case SkillEffectType.Buff:
                // ... 버프 로직, 필요하다면 PlaySkillAnim() 호출
                return true;
        }
        return false;
    }

    public async UniTask ExecuteEnemyAction(Unit attacker, Unit target)
    {
        await ExecuteBasicAttack(attacker, target);
    }
}
