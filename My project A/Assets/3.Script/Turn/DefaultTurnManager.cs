using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(UnitFactory))]
[RequireComponent(typeof(SimpleCombatExecutor))]
[RequireComponent(typeof(InputServiceNew))]
public class DefaultTurnManager : MonoBehaviour
{
    // 플레이어/적 리스트
    private List<PlayerUnit> players;
    private List<EnemyUnit>  enemies;

    // 에디터에서 드래그할 컴포넌트
    [Header("Managers & UI")]
    [SerializeField] private CostManager costManager;
    [SerializeField] private CostBar     costBar;

    // 서비스 컴포넌트
    private InputServiceNew      _inputSvc;
    private SimpleCombatExecutor _executor;
    private UnitFactory          _factory;

    private CancellationTokenSource _cts;

    void Awake()
    {
        // 서비스 초기화
        _inputSvc = GetComponent<InputServiceNew>();
        _executor = GetComponent<SimpleCombatExecutor>();
        _factory  = GetComponent<UnitFactory>();

        // 코스트 풀 초기화 (첫 턴에는 리필 없이 startCost만 세팅)
        costManager.Init(startCost: 4);
        // 코스트 UI 구독
        costBar.Initialize(costManager);
    }

    /// <summary>
    /// GameInitializer에서 플레이어/적 리스트를 할당해주세요.
    /// </summary>
    public void InitializeUnits(List<PlayerUnit> playerList, List<EnemyUnit> enemyList)
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
        bool isFirstRound = true;

        while (!token.IsCancellationRequested)
        {
            // 첫 루프가 아니라면, 이전 턴이 끝난 시점에 리필
            if (!isFirstRound)
                costManager.Refill(1);
            isFirstRound = false;

            // 1) 플레이어 턴 전체
            await PlayerPhase(token);

            // 2) 적 턴 전체
            await EnemyPhase(token);
        }
    }

    private async UniTask PlayerPhase(CancellationToken token)
    {
        foreach (var p in players)
        {
            if (token.IsCancellationRequested) break;

            // 플레이어 유닛을 클릭 → 행동 대기
            var action = await _inputSvc.WaitForPlayerAction(p);

            switch (action.Type)
            {
                case PlayerActionType.BasicAttack:
                    await _executor.ExecuteBasicAttack(p, action.Target);
                    break;

                case PlayerActionType.Skill:
                    int cost = p.SkillData.Cost;
                    if (costManager.Use(cost))
                    {
                        await _executor.ExecuteSkill(p, action.Target);
                        // 스킬은 턴을 종료하지 않습니다!
                        // 따라서 이 foreach 안에서 계속 다음 WaitForPlayerAction을 대기
                        continue;
                    }
                    else
                    {
                        Debug.LogWarning("코스트 부족!");
                        // i-- 같은 처리 없이 그냥 다시 액션선택으로 돌아갑니다.
                        continue;
                    }
            }

            // 일반 공격을 했다면, 즉시 다음 유닛으로 턴 이동
            if (action.Type == PlayerActionType.BasicAttack)
                continue;
        }
    }

    private async UniTask EnemyPhase(CancellationToken token)
    {
        foreach (var e in enemies)
        {
            if (token.IsCancellationRequested) break;
            // 랜덤 타겟 공격 (코스트 무제한)
            var target = players[UnityEngine.Random.Range(0, players.Count)];
            await _executor.ExecuteEnemyAction(e, target);
        }
    }

    void OnDestroy()
    {
        _cts?.Cancel();
    }
}
