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

        bool needMove = attacker.AttackType == AttackRangeType.Melee;

        float moveSpeed = attacker.MoveSpeed;
        float attackOffset = 2f;  // ← 원하는 거리로 직접 조정
        Vector3 attackPos = target.transform.position +
                            new Vector3(attacker.Team == TeamType.Player ? -attackOffset : attackOffset, 0, 0);

        attacker.SetAttackTarget(target);

        if (needMove)
        {
            attacker.LookAt(attackPos);
            await attacker.MoveTo(attackPos, moveSpeed);
        }
        else
        {
            attacker.LookAt(target.transform.position);
        }

        attacker.PlayAttackAnim();

        // 애니메이션 길이 만큼 대기 (임팩트 이벤트에서 데미지)
        float animLen = attacker.GetCurrentAttackAnimLength();
        await UniTask.Delay((int)(animLen * 1000));
        await UniTask.Delay(350);
        
        // (여기서 SetAttackTarget(null) 하지 말 것!)
        if (needMove)
        {
            attacker.LookAt(attacker.SpawnPosition);
            await attacker.MoveToSpawn(moveSpeed);
            attacker.ResetRotation();
        }
        else
        {
            attacker.ResetRotation();
        }
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
