// Scripts/Turn/SimpleCombatExecutor.cs
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 플레이어·적의 공격/스킬 실행을 담당하는 간단한 전투 실행기입니다.
/// </summary>
public class SimpleCombatExecutor : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 일반 공격 처리
    /// </summary>
    public async UniTask ExecuteAttackAsync(PlayerUnit user, EnemyUnit target, CancellationToken ct)
    {
        int damage = Mathf.Max(user.AttackPower - target.Defense, 1);
        target.TakeDamage(damage);
        Debug.Log($"[Attack] {user.name} → {target.name}: {damage} dmg");
        // 300ms 지연, 취소 토큰 적용
        await UniTask.Delay(300, cancellationToken: ct);
    }

    /// <summary>
    /// 플레이어의 스킬 사용 처리
    /// </summary>
    public async UniTask ExecuteSkillAsync(PlayerUnit user, SkillData skill, Component tgt, CancellationToken ct)
    {
        if (tgt is EnemyUnit enemy)
        {
            int damage = skill.Cost * 10;
            enemy.TakeDamage(damage);
            Debug.Log($"[Skill] {user.name} casts {skill.Name} on {enemy.name}: {damage} dmg");
        }
        else if (tgt is PlayerUnit ally)
        {
            int heal = skill.Cost * 10;
            ally.Heal(heal);
            Debug.Log($"[Skill] {user.name} healed {ally.name} for {heal} HP ({ally.CurrentHP}/{ally.MaxHP})");
        }

        await UniTask.Delay(300, cancellationToken: ct);
    }

    /// <summary>
    /// 적의 공격 처리
    /// </summary>
    public async UniTask ExecuteEnemyAttackAsync(EnemyUnit enemy, PlayerUnit target, CancellationToken ct)
    {
        int damage = Mathf.Max(enemy.Attack - target.Defense, 1);
        target.TakeDamage(damage);
        Debug.Log($"[EnemyAttack] {enemy.name} → {target.name}: {damage} dmg");
        await UniTask.Delay(300, cancellationToken: ct);
    }
}