using Cysharp.Threading.Tasks;
using UnityEngine;

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

    public async UniTask ExecuteSkill(Unit attacker, Unit target)
    {
        Debug.Log($"[Combat] {attacker.UnitName} Skill → {target.UnitName}");
        await UniTask.Delay(500);

        // 예시: 스킬은 공격력 2배, 방어력 적용
        int skillDamage = Mathf.Max(0, (attacker.ATK * 2) - target.DEF);
        target.TakeDamage(skillDamage);
    }

    public async UniTask ExecuteEnemyAction(Unit attacker, Unit target)
    {
        Debug.Log($"[Combat] {attacker.UnitName} AI → {target.UnitName}");
        await UniTask.Delay(500);

        int damage = Mathf.Max(0, attacker.ATK - target.DEF);
        target.TakeDamage(damage);
    }
}