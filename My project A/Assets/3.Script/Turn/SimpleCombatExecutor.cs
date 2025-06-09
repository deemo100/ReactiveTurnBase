using System.Threading.Tasks; 
using UnityEngine;

public interface ICombatExecutor
{
    Task ExecuteAttackAsync(Unit attacker, Unit target);
}

public class SimpleCombatExecutor : MonoBehaviour, ICombatExecutor
{
    public async Task ExecuteAttackAsync(Unit attacker, Unit target)
    {
        Debug.Log($"{attacker.unitName} ▶ {target.unitName} 공격!");
        target.TakeDamage(attacker.attackPower);
        await Task.Delay(500);
    }
}