using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input;

/// <summary>
/// 새로운 입력 서비스: 터치/클릭으로 플레이어 행동을 기다리고
/// DefaultTurnManager에 PlayerAction을 반환합니다.
/// </summary>
public class InputServiceNew : MonoBehaviour, IInputService
{
    [Header("Input Actions 에셋")]
    [SerializeField] private InputActionAsset asset;

    private PlayerControls _controls;
    private InputAction _pointer;
    private InputAction _trigger;
    private Camera _cam;
    private UniTaskCompletionSource<PlayerCommand> _tcs;
    private bool _skillMode;

    void Awake()
    {
        // InputActions 래퍼 생성
        _controls = new PlayerControls();
        _cam      = Camera.main;

        // 맵에서 액션 꺼내기
        _pointer = _controls.GamePlay.Pointer;
        _trigger = _controls.GamePlay.Trigger;

        // 클릭/터치 이벤트 연결
        _trigger.performed += ctx => OnTrigger();
    }

    void OnEnable()  => _controls.Enable();
    void OnDisable() => _controls.Disable();

    /// <summary>
    /// 현재 행동할 플레이어 설정 (스킬 모드 초기화 등)
    /// </summary>
    public void SetActivePlayer(PlayerUnit p)
    {
        _skillMode = false;
    }

    /// <summary>
    /// 스킬 버튼 눌러서 다음 클릭을 스킬로 처리하게 합니다.
    /// (예: UI에서 Skill 버튼 눌렀을 때 호출)
    /// </summary>
    public void EnableSkillMode()
    {
        _skillMode = true;
    }

    /// <summary>
    /// DefaultTurnManager가 호출해서
    /// 플레이어 유닛 p가 행동할 때까지 대기한 뒤
    /// PlayerAction을 반환합니다.
    /// </summary>
    public async UniTask<PlayerAction> WaitForPlayerAction(PlayerUnit p)
    {
        // TODO: 실제 클릭/터치 로직으로 대체하세요.
        // 샘플: 매 프레임 대기 후 첫 번째 적을 반환합니다.
        await UniTask.NextFrame();
        var enemy = FindObjectOfType<EnemyUnit>();
        return new PlayerAction {
            Type   = PlayerActionType.BasicAttack,
            Target = enemy
        };
    }

    /// <summary>
    /// (구버전) PlayerCommand를 기다리는 메서드
    /// 필요 없으면 지워도 됩니다.
    /// </summary>
    public UniTask<PlayerCommand> WaitForPlayerCommandAsync(CancellationToken ct)
    {
        _tcs = new UniTaskCompletionSource<PlayerCommand>();
        ct.Register(() => _tcs.TrySetCanceled(), useSynchronizationContext: false);
        return _tcs.Task;
    }

    private void OnTrigger()
    {
        if (_tcs == null)
            return;

        Vector2 sp = _pointer.ReadValue<Vector2>();
        Vector3 wp = _cam.ScreenToWorldPoint(sp);
        wp.z = 0;

        foreach (var col in Physics2D.OverlapPointAll(wp))
        {
            // 스킬 모드에서 아군 클릭
            if (_skillMode && col.TryGetComponent<PlayerUnit>(out var pu))
            {
                _tcs.TrySetResult(new PlayerCommand {
                    IsSkill = true,
                    Skill   = pu.SkillData,
                    Target  = pu
                });
                break;
            }
            // 일반 모드에서 적 클릭
            if (col.TryGetComponent<EnemyUnit>(out var eu))
            {
                _tcs.TrySetResult(new PlayerCommand {
                    IsSkill = _skillMode,
                    Skill   = _skillMode ? /* pu.SkillData */ null : null,
                    Target  = eu
                });
                break;
            }
        }

        // 클릭 후 초기화
        _skillMode = false;
        _tcs = null;
    }
}
