using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Input;
using UnityEngine;

[RequireComponent(typeof(UnitFactory))]
[RequireComponent(typeof(SimpleCombatExecutor))]
[RequireComponent(typeof(InputServiceNew))]
public class DefaultTurnManager : MonoBehaviour
{
    public static DefaultTurnManager Instance { get; private set; } // ⭐싱글톤 추가
    [Header("Cost & UI")]
    [SerializeField] private CostManager costManager;
    [SerializeField] private CostBar     costBar;

    private int turnCount = 0;
    private List<PlayerUnit>     players;
    private List<EnemyUnit>      enemies;
    private bool battleOver = false;
    
    private InputServiceNew      _inputSvc;
    private SimpleCombatExecutor _executor;
    private UnitFactory          _factory;
    private CancellationTokenSource _cts;

    void Awake()
    {
        Instance = this; // ⭐싱글톤 할당
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

            // **여기서 모든 플레이어 유닛의 행동 플래그를 초기화!**
            foreach (var p in players)
            {
                if (!p.IsDead)
                    p.ResetTurn(); // ResetTurn 내부에서 HasActedThisTurn=false 등 초기화
            }

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
    public void CheckVictory()
    {
        if (battleOver) return; // 여러 번 호출 방지

        if (enemies != null && enemies.All(e => e.IsDead))
        {
            battleOver = true;
            Debug.Log("모든 적이 사망했습니다. 승리!");
            UIManager.Instance.ShowVictory();
        }
        else if (players != null && players.All(p => p.IsDead))
        {
            battleOver = true;
            Debug.Log("모든 플레이어가 사망했습니다. 패배...");
            UIManager.Instance.ShowDefeat();
        }
    }

    private async UniTask PlayerPhase(CancellationToken token)
    {
        Debug.Log("플레이어 턴 시작");

        while (players.Any(p => !p.IsDead && !p.HasActedThisTurn))
        {
            var unit = await _inputSvc.WaitForUnitSelect(players.Where(p => !p.IsDead && !p.HasActedThisTurn).ToList());
            Debug.Log($"[플레이어 {unit.UnitName}] 행동 입력 대기");

            while (true)
            {
                var action = await _inputSvc.WaitForPlayerAction(unit);

                // 💡 null 체크 추가!
                bool needTarget =
                    action != null &&
                    action.Type == PlayerActionType.Skill &&
                    (action.SkillData.TargetType == SkillTargetType.EnemySingle ||
                     action.SkillData.TargetType == SkillTargetType.AllySingle ||
                     action.SkillData.TargetType == SkillTargetType.Self);

                if (action == null || (needTarget && action.Target == null))
                {
                    Debug.LogWarning($"[플레이어 {unit.UnitName}] 행동 취소됨 또는 타겟 없음 (다시 선택 가능)");
                    break;
                }
                
                switch (action.Type)
                {
                    case PlayerActionType.BasicAttack:
                        await _executor.ExecuteBasicAttack(unit, action.Target);
                        unit.MarkActed();
                        break;

                    case PlayerActionType.Skill:
                        if (unit.SkillData == null)
                        {
                            Debug.LogError($"[플레이어 {unit.UnitName}] SkillData 없음!");
                            break;
                        }
                        int cost = unit.SkillData.Cost;
                        if (costManager.CanUse(cost))
                        {
                            // 먼저 코스트를 소모하지 말고
                            bool skillSuccess = await _executor.ExecuteSkill(
                                unit,
                                action.Target,
                                unit.SkillData,
                                players,
                                enemies
                            );
                            if (skillSuccess)
                            {
                                costManager.Use(cost); // 성공시에만 코스트 차감!
                                unit.MarkActed();      // 성공시에만 턴 소모!
                            }
                            else
                            {
                                Debug.LogWarning("[Turn] 스킬 사용 실패! 코스트/턴 소모 없음, 재입력 대기");
                                continue; // 다시 행동 선택
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"[플레이어 {unit.UnitName}] 코스트 부족!");
                            continue;
                        }
                        break;
                }
                break;
            }
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
            var alivePlayers = players.Where(p => !p.IsDead).ToList();
            if (alivePlayers.Count == 0)
            {
                Debug.LogWarning("[적 AI] 공격할 수 있는 플레이어가 없습니다.");
                break;
            }
            var target = alivePlayers[UnityEngine.Random.Range(0, alivePlayers.Count)];
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
