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
                if (!_awaitTarget && hit.collider.TryGetComponent<PlayerUnit>(out var playerUnit) && !playerUnit.HasActedThisTurn)
                {
                    if (_selectedUnit != null)
                    {
                        _selectedUnit = playerUnit;
                        playerUnit.SetSelected(true);
                        // **이때만 버튼 활성화**
                        UIManager.Instance.ShowActionButtons(playerUnit);
                    }
                    _selectedUnit = playerUnit;
                    playerUnit.SetSelected(true);
                    Debug.Log($"[선택] {playerUnit.UnitName}이 선택됨");
                    UIManager.Instance.ShowActionButtons(playerUnit);
                    
                    // === 추가 ===
                    // 만약 WaitForUnitSelect() 중이라면 유닛 선택 결과 반환!
                    if (_unitSelectTcs != null && _unitSelectCandidates != null && _unitSelectCandidates.Contains(playerUnit))
                    {
                        _unitSelectTcs.TrySetResult(playerUnit);
                        _unitSelectTcs = null;
                    }
                    
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
        UIManager.Instance.SetAttackHighlight(true); // 공격 효과 활성화
        UIManager.Instance.SetSkillHighlight(false); // 스킬 효과 비활성
    }
    // 스킬 버튼 누를 때 전체 타겟형 처리
    public void EnterSkillMode(SkillData skill)
    {
        _selectedSkill = skill;
        _currentMode = ActionMode.Skill;
        _awaitTarget = true;
        UIManager.Instance.SetAttackHighlight(false); // ⛔️ 공격 하이라이트 꺼두기(겹침 방지)
        UIManager.Instance.SetSkillHighlight(true);   // ✅ 스킬 진입시

        if (skill.TargetType == SkillTargetType.EnemyAll ||
            skill.TargetType == SkillTargetType.AllyAll)
        {
            // 전체형이면 타겟 입력 없이 바로 Action 생성/완료
            TryCompletePlayerAction(new PlayerAction
            {
                Type = PlayerActionType.Skill,
                Actor = _selectedUnit,
                Target = null, // 후처리에서 전체 리스트 참조
                SkillData = skill
            });
        }
    }
    
    public void CancelActionMode()
    {
        _currentMode = ActionMode.None;
        _selectedSkill = null;
        _awaitTarget = false;
        UIManager.Instance.SetAttackHighlight(false);
        UIManager.Instance.SetSkillHighlight(false);
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
        if (_selectedSkill == null || _selectedUnit == null || target == null) return false;

        switch (_selectedSkill.TargetType)
        {
            case SkillTargetType.EnemySingle:
                return target.Team != _selectedUnit.Team;

            case SkillTargetType.AllySingle:
                return target.Team == _selectedUnit.Team; // ← 자기 자신도 OK

            case SkillTargetType.Self:
                return target == _selectedUnit;

            default:
                return false;
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
        UIManager.Instance.SetAttackHighlight(false); // ← 반드시 추가!
    }
}
