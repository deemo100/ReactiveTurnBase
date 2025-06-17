using System;
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
    [Header("Cost & UI")]
    [SerializeField] private CostManager costManager;
    [SerializeField] private CostBar     costBar;

    private int turnCount = 0;
    private bool battleOver = false;

    private List<PlayerUnit>     players;
    private List<EnemyUnit>      enemies;
    private InputServiceNew      _inputSvc;
    private SimpleCombatExecutor _executor;
    private UnitFactory          _factory;
    private CancellationTokenSource _cts;

    void Awake()
    {
        _inputSvc = GetComponent<InputServiceNew>();
        _executor = GetComponent<SimpleCombatExecutor>();
        _factory  = GetComponent<UnitFactory>();

        costBar.Initialize(costManager);      // 1. 먼저 구독!
        costManager.Init(startCost: 4);       // 2. 

        Debug.Log("DefaultTurnManager 초기화 완료");
    }

    public void InitializeUnits(List<PlayerUnit> playerList, List<EnemyUnit> enemyList)
    {
        players = playerList;
        enemies = enemyList;
        Debug.Log($"유닛 초기화: 플레이어 {players.Count}명, 적 {enemies.Count}명");
        PrintAllUnitsState();
    }

    void Start()
    {
        _cts = new CancellationTokenSource();
        Debug.Log("==== 전투 루프 시작 ====");
        RunBattleLoop(_cts.Token).Forget();
    }

    private async UniTask RunBattleLoop(CancellationToken token)
    {
        bool firstRound = true;

        while (!token.IsCancellationRequested && !battleOver)
        {
            turnCount++;
            Debug.Log($"---- {turnCount} 턴 시작 ----");
            UIManager.Instance.UpdateTurnText(turnCount);

            if (!firstRound)
            {
                costManager.Refill(1);
                Debug.Log($"코스트 +1 충전됨 (현재: {costManager.CurrentCost})");
            }
            else
            {
                Debug.Log($"첫 턴, 초기 코스트: {costManager.CurrentCost}");
            }
            firstRound = false;

            PrintAllUnitsState();

            await PlayerPhase(token);
            Debug.Log("플레이어 턴 종료");
            CheckVictory();
            if (battleOver) break;

            await EnemyPhase(token);
            Debug.Log("적 턴 종료");
            CheckVictory();
        }

        Debug.Log("==== 전투 종료 ====");
        PrintAllUnitsState();
    }

    private void CheckVictory()
    {
        if (enemies.All(e => e.IsDead))
        {
            battleOver = true;
            Debug.Log("모든 적이 사망했습니다. 승리!");
            UIManager.Instance.ShowVictory();
        }
        else if (players.All(p => p.IsDead))
        {
            battleOver = true;
            Debug.Log("모든 플레이어가 사망했습니다. 패배...");
            UIManager.Instance.ShowDefeat();
        }
    }

    private async UniTask PlayerPhase(CancellationToken token)
    {
        Debug.Log("[DefaultTurnManager] PlayerPhase 시작");
        _inputSvc.SetPlayerUnits(players);

        while (!_inputSvc.AllPlayerActed())
        {
            Debug.Log("[DefaultTurnManager] 유닛 지정 대기...");
            var selectedUnit = await _inputSvc.WaitForUnitSelect();
            Debug.Log($"[DefaultTurnManager] 선택된 유닛: {selectedUnit.UnitName}");

            var action = await _inputSvc.WaitForPlayerAction(selectedUnit);
            Debug.Log($"[DefaultTurnManager] 행동 타입: {action.Type}, 타겟: {action.Target?.UnitName}");

            // 행동 실행 ... (생략)

            _inputSvc.MarkUnitActed(selectedUnit);
            Debug.Log($"[DefaultTurnManager] {selectedUnit.UnitName} 행동 완료, 다음 선택 가능");
        }
    }

    private async UniTask EnemyPhase(CancellationToken token)
    {
        Debug.Log("적 턴 시작");
        foreach (var e in enemies)
        {
            if (token.IsCancellationRequested) break;
            if (e.IsDead)
            {
                Debug.Log($"[적 {e.UnitName}] 사망 상태로 건너뜀");
                continue;
            }
            var target = players[UnityEngine.Random.Range(0, players.Count)];
            Debug.Log($"[적 {e.UnitName}] → [플레이어 {target.UnitName}] 공격");
            await _executor.ExecuteEnemyAction(e, target);
        }
    }

    void OnDestroy()
    {
        _cts?.Cancel();
        Debug.Log("DefaultTurnManager 종료 (CancellationToken 취소)");
    }

    private void PrintAllUnitsState()
    {
        string playerState = string.Join(", ", players.Select(p => $"{p.UnitName}(HP:{p.HP}/{p.MaxHP}, Dead:{p.IsDead})"));
        string enemyState  = string.Join(", ", enemies.Select(e => $"{e.UnitName}(HP:{e.HP}/{e.MaxHP}, Dead:{e.IsDead})"));
        Debug.Log($"[유닛 상태] 플레이어: {playerState} / 적: {enemyState}");
    }
}
