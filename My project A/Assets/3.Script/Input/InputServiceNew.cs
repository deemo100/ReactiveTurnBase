using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input; 

public class InputServiceNew : MonoBehaviour
{
    [Header("Input Actions 에셋")]
    [SerializeField] private InputActionAsset inputAsset;

    private PlayerControls _controls;
    private InputAction _pointer;
    private InputAction _trigger;
    private Camera _cam;
    private UniTaskCompletionSource<PlayerAction> _tcs;
    private bool _skillMode;
    private PlayerUnit _activePlayer;
    private SkillData _selectedSkill; // 스킬 정보 저장

    void Awake()
    {
        _controls = new PlayerControls();
        _cam = Camera.main;

        _pointer = _controls.GamePlay.Pointer;
        _trigger = _controls.GamePlay.Trigger;

        _trigger.performed += ctx => OnTrigger();

        Debug.Log("[InputServiceNew] Awake 완료");
    }

    void OnEnable()
    {
        _controls.Enable();
        Debug.Log("[InputServiceNew] 입력 시스템 Enable");
    }
    void OnDisable()
    {
        _controls.Disable();
        Debug.Log("[InputServiceNew] 입력 시스템 Disable");
    }

    // 평타(칼) 버튼에서 호출
    public void SelectBasicAttack()
    {
        _skillMode = false;
        _selectedSkill = null;
        Debug.Log("[InputServiceNew] 공격(칼) 모드 진입");
    }

    // 스킬(방패) 버튼에서 호출 (UI에서 사용)
    public void SelectSkill(SkillData skill)
    {
        _skillMode = true;
        _selectedSkill = skill;
        Debug.Log($"[InputServiceNew] 스킬(방패) 모드 진입 - 선택 스킬: {skill?.Name} (타겟타입: {skill?.TargetType})");
    }

    public void SetActivePlayer(PlayerUnit p)
    {
        _activePlayer = p;
        _skillMode = false;
        _selectedSkill = null;
        Debug.Log($"[InputServiceNew] 현재 행동할 플레이어: {p.UnitName}");
    }

    /// <summary>
    /// 플레이어 유닛 p가 행동할 때까지 대기한 뒤 PlayerAction 반환
    /// </summary>
    public async UniTask<PlayerAction> WaitForPlayerAction(PlayerUnit p)
    {
        SetActivePlayer(p);
        _tcs = new UniTaskCompletionSource<PlayerAction>();
        Debug.Log($"[InputServiceNew] {p.UnitName} 행동 입력 대기 시작");
        return await _tcs.Task;
    }

    /// <summary>
    /// 클릭/터치 트리거
    /// </summary>
   private void OnTrigger()
{
    if (_tcs == null) return;

    Vector2 sp = Mouse.current.position.ReadValue();
    Vector3 wp = _cam.ScreenToWorldPoint(new Vector3(sp.x, sp.y, Mathf.Abs(_cam.transform.position.z)));
    wp.z = 0;
    Debug.Log($"[InputServiceNew] 클릭 위치: {wp}");

    foreach (var col in Physics2D.OverlapPointAll(wp))
    {
        // 1. 스킬 모드 (스킬의 TargetType에 따라 분기)
        if (_skillMode && _selectedSkill != null)
        {
            switch (_selectedSkill.TargetType)
            {
                case TargetType.AllyOnly:
                    if (col.TryGetComponent<PlayerUnit>(out var ally))
                    {
                        Debug.Log($"[InputServiceNew] PlayerUnit 클릭됨: {ally.UnitName} (ID: {ally.Id})");
                        TrySetSkillAction(ally);
                        return;
                    }
                    break;
                case TargetType.EnemyOnly:
                    if (col.TryGetComponent<EnemyUnit>(out var enemy))
                    {
                        Debug.Log($"[InputServiceNew] EnemyUnit 클릭됨: {enemy.UnitName} (ID: {enemy.Id})");
                        TrySetSkillAction(enemy);
                        return;
                    }
                    break;
                case TargetType.All:
                    if (col.TryGetComponent<Unit>(out var unit))
                    {
                        Debug.Log($"[InputServiceNew] Unit 클릭됨: {unit.UnitName} (ID: {unit.Id})");
                        TrySetSkillAction(unit);
                        return;
                    }
                    break;
            }
        }
        // 2. 평타(공격) 모드: 적만 타겟팅
        if (!_skillMode && col.TryGetComponent<EnemyUnit>(out var targetEnemy))
        {
            Debug.Log($"[InputServiceNew] 평타로 EnemyUnit 클릭됨: {targetEnemy.UnitName} (ID: {targetEnemy.Id})");
            _tcs.TrySetResult(new PlayerAction
            {
                Type = PlayerActionType.BasicAttack,
                Actor = _activePlayer,
                Target = targetEnemy
            });
            Debug.Log($"[InputServiceNew] 평타 입력 완료: {_activePlayer?.UnitName} -> {targetEnemy.UnitName}");
            EndInput();
            return;
        }

        // --- 추가: 디버깅 용도로, 어떤 콜라이더가 걸렸는지 확인
        Debug.Log($"[InputServiceNew] Overlap된 오브젝트: {col.gameObject.name}");
    }
}

    // 스킬 입력 처리
    private void TrySetSkillAction(Unit target)
    {
        int cost = _selectedSkill.Cost;
        if (_activePlayer == null || _selectedSkill == null)
        {
            Debug.LogWarning("[InputServiceNew] TrySetSkillAction: _activePlayer or _selectedSkill null!");
            return;
        }

        Debug.Log($"[InputServiceNew] 스킬 입력 완료: {_activePlayer.UnitName} ({_selectedSkill.Name}) -> {target.UnitName}");

        _tcs.TrySetResult(new PlayerAction
        {
            Type = PlayerActionType.Skill,
            Actor = _activePlayer,
            Target = target,
            SkillData = _selectedSkill
        });
        EndInput();
    }

    private void EndInput()
    {
        Debug.Log("[InputServiceNew] 입력 종료");
        _skillMode = false;
        _activePlayer = null;
        _selectedSkill = null;
        _tcs = null;
    }
}
