using Cysharp.Threading.Tasks;
using UnityEngine;

public class SimpleCombatExecutor : MonoBehaviour
{
    public async UniTask ExecuteBasicAttack(PlayerUnit attacker, MonoBehaviour target)
    {
        Debug.Log($"[Combat] {attacker.name} BasicAttack → {target.name}");
        await UniTask.Delay(500);

        if (target.TryGetComponent<IDamageable>(out var dmg))
            dmg.TakeDamage(attacker.AttackPower);
    }

    public async UniTask ExecuteSkill(PlayerUnit attacker, MonoBehaviour target)
    {
        Debug.Log($"[Combat] {attacker.name} Skill → {target.name}");
        await UniTask.Delay(500);

        if (target.TryGetComponent<IDamageable>(out var dmg))
            dmg.TakeDamage(attacker.AttackPower * 2);
    }

    public async UniTask ExecuteEnemyAction(EnemyUnit attacker, PlayerUnit target)
    {
        Debug.Log($"[Combat] {attacker.name} AI → {target.name}");
        await UniTask.Delay(500);

        target.TakeDamage(attacker.Attack);
    }
}