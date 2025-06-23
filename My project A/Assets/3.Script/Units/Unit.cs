using Game.Input;
using UnityEngine;
using Cysharp.Threading.Tasks;

public enum TeamType
{
    Player,
    Enemy
}

public enum AttackRangeType
{
    Melee,      // 근접
    Ranged      // 원거리
}

public class Unit : MonoBehaviour
{
    public TeamType Team { get; set; }
    
    public AttackRangeType AttackType = AttackRangeType.Melee;
    
    public Vector3 SpawnPosition { get; private set; }
    public float MoveSpeed = 5f;

    public int Id { get; protected set; }
    public string UnitName { get; protected set; }
    public string ClassName { get; protected set; }
    public int MaxHP { get; protected set; }
    public int HP { get; protected set; }
    public int ATK { get; set; }
    public int DEF { get; protected set; }
    public int MaxGroggy { get; protected set; }
    public int Groggy { get; protected set; }

    public bool IsDead => HP <= 0;
    public bool IsGroggy => Groggy <= 0;

    public HealthBar healthBar;
    public HealthBarFollower healthBarFollower;
    public SkillData SkillData { get; protected set; }

    // 추가: 초기 로테이션 저장
    Quaternion _initialRotation;
    protected virtual void Start()
    {
        SpawnPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    public virtual void Init(UnitStat stat, TeamType team)
    {
        Id = stat.Id;
        UnitName = stat.Name;
        ClassName = stat.ClassName;
        MaxHP = stat.MaxHP;
        HP = stat.MaxHP;
        ATK = stat.Attack;
        DEF = stat.Defense;
        MaxGroggy = stat.MaxGroggy;
        Groggy = stat.MaxGroggy;
        Team = team;
    }

    public virtual void TakeDamage(int amount)
    {
        Debug.Log($"[Unit] TakeDamage 호출됨, {UnitName}, 타입: {this.GetType()}");
        HP = Mathf.Max(0, HP - amount);
        Debug.Log($"[Unit.TakeDamage] {UnitName}, HP: {HP}/{MaxHP}");

        if (healthBarFollower != null)
            healthBarFollower.SetHealth(HP / (float)MaxHP);

        if (HP <= 0)
        {
            DefaultTurnManager.Instance?.CheckVictory();
        }

        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            if (HP > 0)
                animator.SetTrigger("3_Damaged");
            else
                animator.SetTrigger("4_Death");
        }
    }

    // 부드러운 이동
    public async UniTask MoveTo(Vector3 targetPos, float speed = 5f)
    {
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, targetPos);
        float duration = distance / speed;
        float elapsed = 0f;

        // === 1. 애니메이터에서 Move 트리거 On ===
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetBool("1_Move", true);

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }
        transform.position = targetPos;

        // === 2. 이동 끝난 후 Move 트리거 Off ===
        if (animator != null)
            animator.SetBool("1_Move", false);
    }

    public async UniTask MoveToSpawn(float speed = 5f)
    {
        await MoveTo(SpawnPosition, speed);
    }

    // **방향전환**
    public virtual void LookAt(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position);
        if (dir.x > 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);  // 오른쪽
        else
            transform.rotation = Quaternion.Euler(0, 0, 0); // 왼쪽
    }

    // **초기 방향 복구**
    public void ResetRotation()
    {
        transform.rotation = _initialRotation;
    }

    // **애니메이션 길이 자동 획득**
    public float GetCurrentAttackAnimLength()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator == null) return 0.7f; // 기본값

        // 현재 Animator의 모든 AnimationClip 중 "Attack"과 유사한 이름의 첫 클립 반환
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains("attack"))
                return clip.length;
        }
        return 0.7f; // 못 찾으면 기본값
    }
    
    public virtual void PlayAttackAnim()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("2_Attack");
    }

    public virtual void PlaySkillAnim()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("7_Skill");
    }

    public virtual void Heal(int amount)
    {
        HP = Mathf.Min(MaxHP, HP + amount);
        if (healthBarFollower != null)
            healthBarFollower.SetHealth(HP / (float)MaxHP);
    }

    public virtual void TakeGroggy(int amount)
    {
        Groggy = Mathf.Max(0, Groggy - amount);
    }

    public virtual void RecoverGroggy(int amount)
    {
        Groggy = Mathf.Min(MaxGroggy, Groggy + amount);
    }
}
