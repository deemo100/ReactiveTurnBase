using System;
using System.Collections.Generic;
using System.Linq;                   // ★ 이 줄을 추가
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Input;            // ← 이 줄 꼭!

[RequireComponent(typeof(UnitFactory))]
[RequireComponent(typeof(SimpleCombatExecutor))]
[RequireComponent(typeof(InputServiceNew))]
public class DefaultTurnManager : MonoBehaviour
{
    [Header("Cost & UI")]
    [SerializeField] private CostManager costManager;
    [SerializeField] private CostBar     costBar;
    
    private int turnCount = 0;
    private bool  battleOver = false;
    
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

        costManager.Init(startCost: 4);
        costBar.Initialize(costManager);
    }

    public void InitializeUnits(List<PlayerUnit> playerList, List<EnemyUnit> enemyList)
    {
        players = playerList;
        enemies = enemyList;
    }

    void Start()
    {
        _cts = new CancellationTokenSource();
        RunBattleLoop(_cts.Token).Forget();
    }

    private async UniTask RunBattleLoop(CancellationToken token)
    {
        bool firstRound = true;

        while (!token.IsCancellationRequested && !battleOver)
        {
            turnCount++;
            UIManager.Instance.UpdateTurnText(turnCount);  // UI 갱신

            if (!firstRound)
                costManager.Refill(1);
            firstRound = false;

            await PlayerPhase(token);
            CheckVictory();                                  // 플레이어 턴 직후 체크
            if (battleOver) break;

            await EnemyPhase(token);
            CheckVictory();                                  // 적 턴 직후 체크
        }
    }

    private void CheckVictory()
    {
        if (enemies.All(e => e.IsDead))
        {
            battleOver = true;
            UIManager.Instance.ShowVictory();
        }
        else if (players.All(p => p.IsDead))
        {
            battleOver = true;
            UIManager.Instance.ShowDefeat();
        }
    }
    
    private async UniTask PlayerPhase(CancellationToken token)
    {
        foreach (var p in players)
        {
            if (token.IsCancellationRequested) break;

            // ← 여기서 오류 나던 부분
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
                        continue;
                    }
                    else
                    {
                        Debug.LogWarning("코스트 부족!");
                        continue;
                    }
            }

            // 일반 공격이면 다음 유닛으로
            if (action.Type == PlayerActionType.BasicAttack)
                continue;
        }
    }

    private async UniTask EnemyPhase(CancellationToken token)
    {
        foreach (var e in enemies)
        {
            if (token.IsCancellationRequested) break;
            var target = players[UnityEngine.Random.Range(0, players.Count)];
            await _executor.ExecuteEnemyAction(e, target);
        }
    }

    void OnDestroy()
    {
        _cts?.Cancel();
    }
}
