using System.Threading.Tasks;
using UnityEngine;

public interface ISkillExecutor
{
    Task ExecuteAsync(SkillData skill, Unit caster, Unit target);
}

public class SimpleSkillExecutor : MonoBehaviour, ISkillExecutor
{
    public async Task ExecuteAsync(SkillData skill, Unit caster, Unit target)
    {
        Debug.Log($"{caster.unitName} ▶ {skill.name} 사용 → {target.unitName}");
        // 예시 연출: 공격력 * cost 비례 대미지
        int dmg = caster.attackPower * skill.cost;
        target.TakeDamage(dmg);
        await Task.Delay(500);
    }
}