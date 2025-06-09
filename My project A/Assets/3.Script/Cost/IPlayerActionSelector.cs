using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPlayerActionSelector
{
    // 기존
    Task<PlayerAction> SelectActionAsync(Unit caster);

    // ↓ 아래 메서드를 반드시 선언해야 DefaultTurnManager에서 호출할 수 있습니다.
    Task<Unit> SelectTargetAsync(IEnumerable<Unit> candidates);
}