// Scripts/Turn/SimpleCombatExecutor.cs
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SimpleCombatExecutor : MonoBehaviour
{
    public async UniTask ExecuteAttackAsync(PlayerUnit user, EnemyUnit target, CancellationToken token)
    {
        int damage = Mathf.Max(user.AttackPower - target.Defense, 1);
        target.CurrentHP -= damage;
        Debug.Log($"[Damage] {user.name} attacked {target.name} for {damage} dmg. ({target.CurrentHP}/{target.MaxHP} HP left)");
        await UniTask.Delay(500, cancellationToken: token);
    }

    public async UniTask ExecuteSkillAsync(PlayerUnit user, SkillData skill, Component tgt, CancellationToken token)
    {
        if (tgt is EnemyUnit enemy)
        {
            int damage = skill.Cost * 10;
            enemy.CurrentHP -= damage;
            Debug.Log($"[Skill] {user.name} cast {skill.Name} on {enemy.name} for {damage} dmg. ({enemy.CurrentHP}/{enemy.MaxHP} HP left)");
        }
        else if (tgt is PlayerUnit ally)
        {
            int heal = skill.Cost * 10;
            int before = ally.CurrentHP;
            ally.CurrentHP = Mathf.Min(ally.CurrentHP + heal, ally.MaxHP);
            Debug.Log($"[Skill] {user.name} cast {skill.Name} on {ally.name}: healed {ally.CurrentHP - before} HP. ({ally.CurrentHP}/{ally.MaxHP})");
        }
        await UniTask.Delay(700, cancellationToken: token);
    }

    public async UniTask ExecuteEnemyAttackAsync(EnemyUnit enemy, PlayerUnit target, CancellationToken token)
    {
        int damage = Mathf.Max(enemy.Attack - target.Defense, 1);
        target.CurrentHP -= damage;
        Debug.Log($"[Damage] {enemy.name} attacked {target.name} for {damage} dmg. ({target.CurrentHP}/{target.MaxHP} HP left)");
        await UniTask.Delay(500, cancellationToken: token);
    }
}