// Scripts/Input/InputServiceMono.cs
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class InputServiceMono : MonoBehaviour, IInputService
{
    [Header("UI Buttons")]
    [SerializeField] private Button attackBtn;
    [SerializeField] private Button skillBtn;
    [SerializeField] private Image  skillIconImage;

    [Header("Target Selector")]
    [SerializeField] private PlayerClickTargetSelector selector;

    private PlayerUnit activePlayer;
    private bool       skillMode;
    private UniTaskCompletionSource<PlayerCommand> tcs;

    void Awake()
    {
        attackBtn.onClick.AddListener(() => SetSkillMode(false));
        skillBtn.onClick.AddListener(ToggleSkillMode);

        selector.OnEnemyClicked .AddListener(OnEnemyClick);
        selector.OnPlayerClicked.AddListener(OnPlayerClick);
    }

    // IInputService implementation
    public UniTask<PlayerCommand> WaitForPlayerCommandAsync(CancellationToken ct)
    {
        tcs = new UniTaskCompletionSource<PlayerCommand>();
        ct.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
        return tcs.Task;
    }

    /// <summary>
    /// 이번 턴 처리할 플레이어 유닛을 설정합니다.
    /// </summary>
    public void SetActivePlayer(PlayerUnit p)
    {
        activePlayer = p;
        // UI에 해당 유닛 스킬 아이콘 갱신
        var icon = Resources.Load<Sprite>($"Icons/{p.SkillData.IconName}");
        skillIconImage.sprite = icon;

        // 턴 시작 시 항상 일반 공격 모드로 초기화
        SetSkillMode(false);
    }

    private void ToggleSkillMode()
    {
        // 스킬 대기 중이면 취소, 아니면 대기
        SetSkillMode(!skillMode);
    }

    private void SetSkillMode(bool on)
    {
        skillMode = on;
        // 버튼 하이라이트 처리 예시
        skillBtn.image.color = skillMode ? Color.yellow : Color.white;
    }

    public void OnEnemyClick(EnemyUnit enemy)
    {
        if (tcs == null)
            return;

        Debug.Log($"[Click] Enemy clicked: {enemy.name}");  // ← 여기에 클릭 로그
        
        if (skillMode)
        {
            // 스킬 모드에서 적 클릭 → 스킬 발동
            tcs.TrySetResult(new PlayerCommand
            {
                IsSkill = true,
                Skill   = activePlayer.SkillData,
                Target  = enemy
            });
        }
        else
        {
            // 일반 공격 모드에서 적 클릭 → 공격
            tcs.TrySetResult(new PlayerCommand
            {
                IsSkill = false,
                Target  = enemy
            });
        }

        EndCommand();
    }

    public void OnPlayerClick(PlayerUnit player)
    {
        if (tcs == null || !skillMode)
            return;

        Debug.Log($"[Click] Player clicked: {player.name}"); 
        
        // 스킬 모드에서 아군 클릭 → 스킬 발동
        activePlayer = player;
        skillIconImage.sprite = Resources.Load<Sprite>($"Icons/{player.SkillData.IconName}");

        tcs.TrySetResult(new PlayerCommand
        {
            IsSkill = true,
            Skill   = player.SkillData,
            Target  = player
        });

        EndCommand();
    }

    private void EndCommand()
    {
        tcs = null;
        SetSkillMode(false);
    }
}
