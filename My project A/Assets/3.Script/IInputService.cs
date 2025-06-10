// Scripts/Turn/Interfaces/IInputService.cs
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 플레이어의 공격/스킬 입력 흐름을 정의하는 인터페이스
/// </summary>
public interface IInputService
{
    /// <summary>
    /// 플레이어가 공격 혹은 스킬+대상을 선택할 때까지 대기
    /// </summary>
    UniTask<PlayerCommand> WaitForPlayerCommandAsync(CancellationToken ct);

    /// <summary>
    /// 이번 턴에 동작할 플레이어 유닛을 알려줍니다.
    /// </summary>
    void SetActivePlayer(PlayerUnit player);
}