using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.Input;

public class SimpleCombatExecutor : MonoBehaviour
{
    public async UniTask ExecuteBasicAttack(Unit attacker, Unit target)
    {
        if (attacker is PlayerUnit pu) pu.SetBusy(true);
        if (target == null || target.IsDead) return;

        bool needMove = attacker.AttackType == AttackRangeType.Melee;
        float moveSpeed = attacker.MoveSpeed;
        float attackOffset = 2f;
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

        float animLen = attacker.GetCurrentAttackAnimLength();
        await UniTask.Delay((int)(animLen * 1000));
        await UniTask.Delay(350);

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
        if (attacker is PlayerUnit pu2) pu2.SetBusy(false); // ★추가
    }

    public async UniTask<bool> ExecuteSkill(
        PlayerUnit actor,
        Unit target,
        SkillData skill,
        List<PlayerUnit> allPlayers,
        List<EnemyUnit> allEnemies)
    {
        actor.SetBusy(true); // ★추가
        float moveSpeed = actor.MoveSpeed;
        float skillOffset = 2f;

        // 타겟 분기(전체/단일)
        switch (skill.TargetType)
        {
            case SkillTargetType.EnemyAll:
                actor.SetAttackTargets(allEnemies.Cast<Unit>().ToList());
                break;
            case SkillTargetType.AllyAll:
                actor.SetAttackTargets(allPlayers.Cast<Unit>().ToList());
                break;
            case SkillTargetType.EnemySingle:
            case SkillTargetType.AllySingle:
            case SkillTargetType.Self:
                actor.SetAttackTarget(target);
                break;
        }

        bool isMelee = skill.CastType == SkillCastType.Melee;
        if (isMelee && target != null)
        {
            Vector3 skillAttackPos = target.transform.position +
                new Vector3(actor.Team == TeamType.Player ? -skillOffset : skillOffset, 0, 0);
            actor.LookAt(skillAttackPos);
            await actor.MoveTo(skillAttackPos, moveSpeed);
        }
        else if (target != null)
        {
            actor.LookAt(target.transform.position);
        }

        actor.PlaySkillAnim();

        float animLen = actor.GetCurrentAttackAnimLength();
        await UniTask.Delay((int)(animLen * 1000));
        await UniTask.Delay(350);

        if (isMelee && target != null)
        {
            actor.LookAt(actor.SpawnPosition);
            await actor.MoveToSpawn(moveSpeed);
            actor.ResetRotation();
        }
        else
        {
            actor.ResetRotation();
        }
        actor.SetBusy(false); // ★추가
        return true;
    }

    public async UniTask ExecuteEnemyAction(Unit attacker, Unit target)
    {
        await ExecuteBasicAttack(attacker, target);
    }
}
