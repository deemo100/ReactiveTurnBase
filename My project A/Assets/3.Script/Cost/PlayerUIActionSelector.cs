using System;
using System.Collections.Generic;    // IEnumerable<T>
using System.Threading.Tasks;       // TaskCompletionSource, Task
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIActionSelector : MonoBehaviour, IPlayerActionSelector
{
    [Header("UI 버튼")]
    public Button AttackButton;
    public SkillSlotUI[] SkillSlots;

    // 클릭 대기를 위해 재사용
    private TaskCompletionSource<PlayerAction> _actionTcs;
    private TaskCompletionSource<Unit>         _targetTcs;

    private void Awake()
    {
        AttackButton.onClick.AddListener(OnAttackClicked);
        foreach (var slot in SkillSlots)
            slot.OnClicked += OnSkillClicked;
    }

    private void OnAttackClicked()
    {
        _actionTcs?.TrySetResult(new PlayerAction { Type = ActionType.BasicAttack });
    }

    private void OnSkillClicked(SkillData skill)
    {
        _actionTcs?.TrySetResult(new PlayerAction { Type = ActionType.Skill, Skill = skill });
    }

    // 1) 액션 선택 (공격 vs 스킬)
    public Task<PlayerAction> SelectActionAsync(Unit caster)
    {
        _actionTcs = new TaskCompletionSource<PlayerAction>();
        return _actionTcs.Task.ContinueWith(t =>
        {
            var a = t.Result;
            _actionTcs = null;
            return a;
        });
    }

    // 2) 대상 선택 (적 클릭 대기)
    public Task<Unit> SelectTargetAsync(IEnumerable<Unit> candidates)
    {
        _targetTcs = new TaskCompletionSource<Unit>();
        // Unit.OnUnitClicked 은 Unit.cs 에서 호출되는 이벤트입니다.
        Unit.OnUnitClicked += OnUnitClicked;
        return _targetTcs.Task.ContinueWith(t =>
        {
            var u = t.Result;
            // 이벤트 해제
            Unit.OnUnitClicked -= OnUnitClicked;
            _targetTcs = null;
            return u;
        });

        void OnUnitClicked(Unit u)
        {
            if (candidates != null && System.Linq.Enumerable.Contains(candidates, u))
                _targetTcs.TrySetResult(u);
        }
    }
}