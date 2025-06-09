using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class DefaultTurnManager : MonoBehaviour
{
    [Header("유닛")]
    public Unit[] playerUnits;
    public Unit[] enemyUnits;

    [Header("Cost / UI")]
    public ICostManager        CostManager;   // SimpleCostManager 연결
    public CostDisplayUI       CostDisplay;   // CostSet UI
    public IPopupService       PopupService;  // 팝업

    [Header("액션 선택")]
    public IPlayerActionSelector ActionSelector;

    [Header("공격 실행")]
    public ICombatExecutor     CombatExecutor;

    [Header("스킬 실행")]
    public ISkillExecutor      SkillExecutor;

    [Header("턴 UI")]
    public TMP_Text turnText;
    private int _turn = 1;

    private async void Start()
    {
        CostDisplay.UpdateDisplay(CostManager.CurrentCost);
        UpdateTurnUI();
        await RunBattleAsync();
    }

    private void UpdateTurnUI()
    {
        turnText.text = $"Turn: {_turn}";
    }

    private async Task RunBattleAsync()
    {
        Debug.Log("전투 시작");
        while (playerUnits.Any(p => p.currentHP > 0) && enemyUnits.Any(e => e.currentHP > 0))
        {
            // 플레이어 턴
            foreach (var p in playerUnits.Where(p => p.currentHP > 0))
            {
                // 스턴 체크
                if (p.IsStunned)
                {
                    p.DecreaseStunTurn();
                    continue;
                }

                // 1) 유저 액션 선택
                var action = await ActionSelector.SelectActionAsync(p);

                // 2) 비용/행동 처리
                if (action.Type == ActionType.BasicAttack)
                {
                    // 기본 공격은 코스트 +1 회복
                    CostManager.Recover(1);
                    CostDisplay.UpdateDisplay(CostManager.CurrentCost);
                    var target = await ActionSelector.SelectTargetAsync(enemyUnits);
                    await CombatExecutor.ExecuteAttackAsync(p, target);
                }
                else // Skill
                {
                    var skill = action.Skill;
                    if (!CostManager.Spend(skill.cost))
                    {
                        PopupService.Show("코스트 부족!");
                        // 다시 선택하게 continue
                        p.DecreaseStunTurn();  
                        goto PlayerNext; 
                    }
                    CostDisplay.UpdateDisplay(CostManager.CurrentCost);

                    var target = await ChooseTargetAsync(p, skill);
                    await SkillExecutor.ExecuteAsync(skill, p, target);
                }

                PlayerNext:
                p.DecreaseStunTurn();
            }

            // 적 턴 (기존처럼 자동)
            foreach (var e in enemyUnits.Where(e => e.currentHP > 0))
            {
                if (e.IsStunned)
                {
                    e.DecreaseStunTurn();
                    continue;
                }

                var tgt = playerUnits.First(p => p.currentHP > 0);
                await CombatExecutor.ExecuteAttackAsync(e, tgt);
                e.DecreaseStunTurn();
            }

            // 턴 종료
            _turn++;
            UpdateTurnUI();
        }

        Debug.Log(playerUnits.Any(p => p.currentHP > 0) ? "승리!" : "패배!");
    }

    private Task<Unit> ChooseTargetAsync(Unit caster, SkillData skill)
    {
        // EnemyOnly면 클릭 대기, Self/AllyOnly 등 추가 가능
        if (skill.targetType == TargetType.EnemyOnly)
            return (ActionSelector as PlayerUIActionSelector)
                   .SelectTargetAsync(enemyUnits);
        return Task.FromResult(caster);
    }
}