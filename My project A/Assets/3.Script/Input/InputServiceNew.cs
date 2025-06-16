using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

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

    void Awake()
    {
        _controls = new PlayerControls();
        _cam = Camera.main;

        _pointer = _controls.GamePlay.Pointer;
        _trigger = _controls.GamePlay.Trigger;

        _trigger.performed += ctx => OnTrigger();
    }

    void OnEnable() => _controls.Enable();
    void OnDisable() => _controls.Disable();

    /// <summary>
    /// 현재 행동할 플레이어 설정 (스킬 모드 초기화 등)
    /// </summary>
    public void SetActivePlayer(PlayerUnit p)
    {
        _activePlayer = p;
        _skillMode = false;
    }

    /// <summary>
    /// UI에서 Skill 버튼 눌렀을 때 호출
    /// </summary>
    public void EnableSkillMode()
    {
        _skillMode = true;
    }

    /// <summary>
    /// 플레이어 유닛 p가 행동할 때까지 대기한 뒤 PlayerAction 반환
    /// </summary>
    public async UniTask<PlayerAction> WaitForPlayerAction(PlayerUnit p)
    {
        SetActivePlayer(p);
        _tcs = new UniTaskCompletionSource<PlayerAction>();
        return await _tcs.Task;
    }

    /// <summary>
    /// 실제 클릭/터치 트리거 처리
    /// </summary>
    private void OnTrigger()
    {
        if (_tcs == null) return;

        Vector2 sp = _pointer.ReadValue<Vector2>();
        Vector3 wp = _cam.ScreenToWorldPoint(sp);
        wp.z = 0;

        // 타겟 선택
        foreach (var col in Physics2D.OverlapPointAll(wp))
        {
            // 스킬 모드(아군 타겟 가능)
            if (_skillMode && col.TryGetComponent<PlayerUnit>(out var ally))
            {
                _tcs.TrySetResult(new PlayerAction
                {
                    Type = PlayerActionType.Skill,
                    Actor = _activePlayer,
                    Target = ally
                });
                EndInput();
                return;
            }
            // 일반 공격 모드(적 타겟)
            if (!_skillMode && col.TryGetComponent<EnemyUnit>(out var enemy))
            {
                _tcs.TrySetResult(new PlayerAction
                {
                    Type = PlayerActionType.BasicAttack,
                    Actor = _activePlayer,
                    Target = enemy
                });
                EndInput();
                return;
            }
        }
    }

    private void EndInput()
    {
        _skillMode = false;
        _activePlayer = null;
        _tcs = null;
    }
}
