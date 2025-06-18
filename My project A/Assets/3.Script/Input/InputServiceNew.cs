using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using Game.Input;

public enum ActionMode { None, Attack, Skill }

public class InputServiceNew : MonoBehaviour
{
    
    private UniTaskCompletionSource<PlayerUnit> _unitSelectTcs;
    private List<PlayerUnit> _unitSelectCandidates;
    
    public static InputServiceNew Instance { get; private set; }

    private PlayerUnit _selectedUnit;
    private ActionMode _currentMode = ActionMode.None;
    private bool _awaitTarget = false;
    private SkillData _selectedSkill;

    private UniTaskCompletionSource<PlayerAction> _actionTcs;

    void Awake()
    {
        Instance = this;
    }

    public async UniTask<PlayerUnit> WaitForUnitSelect(List<PlayerUnit> candidates)
    {
        Debug.Log("[InputServiceNew] WaitForUnitSelect 진입, 행동 가능한 유닛 선택 대기 중...");
        _unitSelectCandidates = candidates;
        _unitSelectTcs = new UniTaskCompletionSource<PlayerUnit>();
        return await _unitSelectTcs.Task;
    }
    
    void Update()
    {
        if (Mouse.current == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 screenPos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // --- 공격/스킬 명령 입력 상태 ---
                if (_awaitTarget)
                {
                    // 1. 공격 모드 - EnemyUnit만 허용
                    if (_currentMode == ActionMode.Attack && hit.collider.TryGetComponent<EnemyUnit>(out var enemy))
                    {
                        Debug.Log($"{_selectedUnit.UnitName}이 {enemy.UnitName}을 공격!");
                        TryCompletePlayerAction(new PlayerAction
                        {
                            Type = PlayerActionType.BasicAttack,
                            Actor = _selectedUnit,
                            Target = enemy
                        });
                        return;
                    }
                    // 2. 스킬 모드 - 타겟 타입에 따라 분기
                    if (_currentMode == ActionMode.Skill && hit.collider.TryGetComponent<Unit>(out var unit) && IsSkillTargetValid(unit))
                    {
                        Debug.Log($"{_selectedUnit.UnitName}이 {unit.UnitName}에게 스킬({_selectedSkill.Name}) 사용!");
                        TryCompletePlayerAction(new PlayerAction
                        {
                            Type = PlayerActionType.Skill,
                            Actor = _selectedUnit,
                            Target = unit,
                            SkillData = _selectedSkill
                        });
                        return;
                    }
                    // 3. 타겟이 아니면(빈 화면 클릭): 행동 명령 취소
                    Debug.Log("빈 화면 클릭 - 행동 명령 취소");
                    CancelActionMode();
                    DeselectCurrentUnit();
                    UIManager.Instance.HideActionButtons();
                    // 실패 시 Task도 해제 필요(옵션)
                    TryCompletePlayerAction(null); // null로 해제(실패 취급)
                    return;
                }

                // --- 평상시 유닛 선택 ---
                if (hit.collider.TryGetComponent<PlayerUnit>(out var playerUnit) && !playerUnit.HasActedThisTurn)
                {
                    if (_selectedUnit != null)
                    {
                        Debug.Log("[InputServiceNew] 빈 화면 클릭 - 유닛 선택 해제/액션 버튼 숨김");
                        _selectedUnit.SetSelected(false);
                        _selectedUnit = null;
                        UIManager.Instance.HideActionButtons();
                    }
                    _selectedUnit = playerUnit;
                    playerUnit.SetSelected(true);
                    Debug.Log($"[선택] {playerUnit.UnitName}이 선택됨");
                    UIManager.Instance.ShowActionButtons(playerUnit);
                }
            }
            else
            {
                // 아무 오브젝트도 클릭되지 않은(=빈 화면 클릭) 경우: 유닛 선택 해제
                if (!_awaitTarget && _selectedUnit != null)
                {
                    Debug.Log("빈 화면 클릭 - 유닛 선택 해제");
                    DeselectCurrentUnit();
                    UIManager.Instance.HideActionButtons();
                }
                // 행동 명령 모드 중이면, 위에서 이미 처리됨
            }
        }
    }

    // --- 외부에서 호출(버튼 등) ---
    public void EnterAttackMode()
    {
        if (_selectedUnit == null) return;
        _currentMode = ActionMode.Attack;
        _awaitTarget = true;
    }
    public void EnterSkillMode(SkillData skill)
    {
        if (_selectedUnit == null) return;
        _currentMode = ActionMode.Skill;
        _selectedSkill = skill;
        _awaitTarget = true;
    }
    public void CancelActionMode()
    {
        _currentMode = ActionMode.None;
        _selectedSkill = null;
        _awaitTarget = false;
    }
    private void DeselectCurrentUnit()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.SetSelected(false);
            _selectedUnit = null;
        }
    }
    private bool IsSkillTargetValid(Unit target)
    {
        if (_selectedSkill == null) return false;
        switch (_selectedSkill.TargetType)
        {
            case TargetType.AllyOnly: return target is PlayerUnit;
            case TargetType.EnemyOnly: return target is EnemyUnit;
            case TargetType.All: return true;
            default: return false;
        }
    }

    // ✅ PlayerPhase에서 호출 (반드시 필요!)
    public async UniTask<PlayerAction> WaitForPlayerAction(PlayerUnit p)
    {
        Debug.Log($"[InputServiceNew] WaitForPlayerAction 진입: {p.UnitName}");
        _selectedUnit = p;
        _currentMode = ActionMode.None;
        _selectedSkill = null;
        _awaitTarget = false;
        _actionTcs = new UniTaskCompletionSource<PlayerAction>();
        // 버튼 클릭(EnterAttackMode/EnterSkillMode) → 타겟 클릭 → TryCompletePlayerAction()에서 SetResult!
        return await _actionTcs.Task;
    }

    // 실제 행동 입력시 결과 전달
    private void TryCompletePlayerAction(PlayerAction action)
    {
        _actionTcs?.TrySetResult(action);
        _selectedUnit?.SetSelected(false);
        _selectedUnit = null;
        _currentMode = ActionMode.None;
        _selectedSkill = null;
        _awaitTarget = false;
        UIManager.Instance.HideActionButtons();
    }
}
