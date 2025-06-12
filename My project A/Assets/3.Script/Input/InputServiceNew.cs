// Assets/3.Script/Input/InputServiceNew.cs
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;

public class InputServiceNew : MonoBehaviour, IInputService
{
    
    [Header("Input Actions 에셋")]
    [SerializeField] private InputActionAsset asset;
    
    PlayerControls _controls;
    InputAction    _pointer, _trigger;
    Camera         _cam;
    UniTaskCompletionSource<PlayerCommand> _tcs;
    bool           _skillMode;

    void Awake()
    {
        // 파라미터 없이 래퍼 생성
        _controls = new PlayerControls();
        _cam      = Camera.main;

        // 액션 맵 바인딩
        _pointer = _controls.GamePlay.Pointer;
        _trigger = _controls.GamePlay.Trigger;

        // 클릭/터치 콜백
        _trigger.performed += ctx => OnTrigger();
    }

    void OnEnable()  => _controls.Enable();
    void OnDisable() => _controls.Disable();

    public void SetActivePlayer(PlayerUnit p)
    {
        _skillMode = false;
    }

    public UniTask<PlayerCommand> WaitForPlayerCommandAsync(CancellationToken ct)
    {
        _tcs = new UniTaskCompletionSource<PlayerCommand>();
        ct.Register(() => _tcs.TrySetCanceled(), useSynchronizationContext:false);
        return _tcs.Task;
    }

    private void OnTrigger()
    {
        if (_tcs == null) return;

        Vector2 sp = _pointer.ReadValue<Vector2>();
        Vector3 wp = _cam.ScreenToWorldPoint(sp); wp.z = 0;

        foreach (var col in Physics2D.OverlapPointAll(wp))
        {
            if (_skillMode && col.TryGetComponent<PlayerUnit>(out var pu))
            {
                _tcs.TrySetResult(new PlayerCommand {
                    IsSkill = true,
                    Skill   = pu.SkillData,
                    Target  = pu
                });
                break;
            }
            if (col.TryGetComponent<EnemyUnit>(out var eu))
            {
                _tcs.TrySetResult(new PlayerCommand {
                    IsSkill = _skillMode,
                    Skill   = _skillMode ? null : null,
                    Target  = eu
                });
                break;
            }
        }

        _skillMode = false;
        _tcs = null;
    }
}