using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DefaultTurnManager : MonoBehaviour
{
    [SerializeField] private List<PlayerUnit>     players;
    [SerializeField] private List<EnemyUnit>      enemies;

    [SerializeField] private InputServiceMono     inputService;
    [SerializeField] private SimpleCombatExecutor executor;
    [SerializeField] private UnitFactory          factory;
    
    private CancellationTokenSource cts;

// DefaultTurnManager.cs 의 Start() 예시
    void Start()
    {
        cts = new CancellationTokenSource();

        // 1,2,3,4는 statId 예시
        players = new List<PlayerUnit>() {
            factory.CreatePlayer(1, new Vector3(-2,0,0)),
            factory.CreatePlayer(2, new Vector3(-1,0,0)),
            // ...
        };
        enemies = new List<EnemyUnit>() {
            factory.CreateEnemy(10, new Vector3(2,0,0)),
            // ...
        };

        Debug.Log($"[BattleLoop] Initialized with {players.Count} players and {enemies.Count} enemies");
        RunBattleLoop(cts.Token).Forget();
    }
    
    private async UniTask RunBattleLoop(CancellationToken token)
    {
        int turn = 0;
        while (!token.IsCancellationRequested
               && players.Any(p => !p.IsDead)
               && enemies.Any(e => !e.IsDead))
        {
            turn++;
            Debug.Log($"[BattleLoop] --- Turn {turn} start ---");

            // 1) 플레이어 턴
            var player = players.First(p => !p.IsDead);
            Debug.Log($"[BattleLoop] Player turn: {player.name}");
            inputService.SetActivePlayer(player);

            Debug.Log("[BattleLoop] Waiting for player command...");
            var cmd = await inputService.WaitForPlayerCommandAsync(token);
            Debug.Log($"[BattleLoop] Received Command → IsSkill:{cmd.IsSkill}, Target:{cmd.Target.name}");

            if (cmd.IsSkill)
                await executor.ExecuteSkillAsync(player, cmd.Skill, (Component)cmd.Target, token);
            else
                await executor.ExecuteAttackAsync(player, (EnemyUnit)cmd.Target, token);

            // 2) 적 턴
            var enemy = enemies.First(e => !e.IsDead);
            Debug.Log($"[BattleLoop] Enemy turn: {enemy.name}");
            var target = players.First(p => !p.IsDead);
            await executor.ExecuteEnemyAttackAsync(enemy, target, token);

            Debug.Log($"[BattleLoop] --- Turn {turn} end ---");
        }

        Debug.Log("[BattleLoop] Battle ended");
    }

    void OnDestroy()
    {
        cts?.Cancel();
    }
}