using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(UnitFactory))]
[RequireComponent(typeof(SimpleCombatExecutor))]
[RequireComponent(typeof(InputServiceNew))]
public class DefaultTurnManager : MonoBehaviour
{
    // 필드 (private으로 선언했으면 변경 없이 여기 이름 그대로 씁니다)
    private List<PlayerUnit>      players;
    private List<EnemyUnit>       enemies;
    private InputServiceNew       _inputSvc;
    private SimpleCombatExecutor  _executor;
    private UnitFactory           _factory;
    private CancellationTokenSource _cts;

    void Awake()
    {
        _inputSvc = GetComponent<InputServiceNew>();
        _executor = GetComponent<SimpleCombatExecutor>();
        _factory  = GetComponent<UnitFactory>();
    }

    /// <summary>
    /// GameInitializer에서 Awake() 중 호출해서
    /// players/enemies 리스트를 할당해 줍니다.
    /// </summary>
    public void InitializeUnits(List<PlayerUnit> playerList,
        List<EnemyUnit>  enemyList)
    {
        players = playerList;
        enemies = enemyList;
        Debug.Log($"[Init] players={players.Count}, enemies={enemies.Count}");
    }

    void Start()
    {
        _cts = new CancellationTokenSource();
        RunBattleLoop(_cts.Token).Forget();
    }

    private async UniTask RunBattleLoop(CancellationToken token)
    {
        // 루프 로직...
    }

    void OnDestroy()
    {
        _cts?.Cancel();
    }
}