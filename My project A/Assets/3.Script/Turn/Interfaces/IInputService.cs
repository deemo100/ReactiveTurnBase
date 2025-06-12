// Assets/Scripts/Turn/Interfaces/IInputService.cs
using Cysharp.Threading.Tasks;
using System.Threading;

public interface IInputService
{
    // 매 턴 시작할 플레이어를 알려주고
    void SetActivePlayer(PlayerUnit p);

    // 플레이어 커맨드를 대기 후 반환
    UniTask<PlayerCommand> WaitForPlayerCommandAsync(CancellationToken ct);
}