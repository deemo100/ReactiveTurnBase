using Game.Input;
using UnityEngine;
using Cysharp.Threading.Tasks;

public enum TeamType { Player, Enemy }
public enum AttackRangeType { Melee, Ranged }

public class Unit : MonoBehaviour
{
    public TeamType Team { get; set; }
    public AttackRangeType AttackType = AttackRangeType.Melee;

    [Header("원거리 유닛만 설정")]
    public GameObject arrowPrefab;
    public GameObject fireballPrefab;
    public Transform firePoint;
    public Transform firePointfireball;
    public Vector3 SpawnPosition { get; private set; }
    public float MoveSpeed = 10f;
    public int Id { get; protected set; }
    public string UnitName { get; protected set; }
    public int MaxHP { get; protected set; }
    public int HP { get; protected set; }
    public int ATK { get; set; }
    public int DEF { get; protected set; }

    public bool IsDead => HP <= 0;
    public HealthBar healthBar;
    public HealthBarFollower healthBarFollower;
    public SkillData SkillData { get; protected set; }

    protected Unit _currentTarget; // 애니메이션 이벤트용(현재 공격 타겟)
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
        MaxHP = stat.MaxHP;
        HP = stat.MaxHP;
        ATK = stat.Attack;
        DEF = stat.Defense;
        Team = team;
    }

    // 타겟 지정/초기화
    public void SetAttackTarget(Unit t)
    {
        Debug.Log($"[SetAttackTarget] {name} | 타겟을 {t?.UnitName}로 세팅");
        _currentTarget = t;
        Debug.Log($"[SetAttackTarget] {this.name} | 타겟 오브젝트:{t?.name ?? "null"}, " +
                  $"타입:{t?.GetType().Name ?? "null"}, 유닛명:{t?.UnitName ?? "null"}");
    }

    // 애니메이션 이벤트에서 호출!
    public virtual void OnAttackImpact()
    {
        if (_currentTarget != null)
        {
            Debug.Log($"[이벤트] OnAttackImpact! 현재 타겟: {_currentTarget?.UnitName}");
            _currentTarget.PlayDamagedAnim();
            int damage = Mathf.Max(0, ATK - _currentTarget.DEF);
            _currentTarget.TakeDamage(damage);
            _currentTarget = null; // ← 임팩트 이후 null로 해제!
        }
        else
        {
            Debug.LogWarning("[이벤트] OnAttackImpact: _currentTarget이 null입니다!");
        }
    }

    // 애니메이션에서 호출하는 화살 발사 함수
    public void FireArrowFX()
    {
        if (AttackType != AttackRangeType.Ranged) return;
        if (arrowPrefab == null || firePoint == null || _currentTarget == null) return;

        // === 여기서 애니메이션에서 두 이벤트 사이 시간만큼 비행하게 설정 ===
        float arrowFlyTime = 0.33f; // 실제 이벤트(프레임) 간 시간(초 단위)로 설정!

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrowScript = arrowObj.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.SetTarget(_currentTarget.SpawnPosition, arrowFlyTime);
    }
    
    public void FireFireballFX()
    {
        if (AttackType != AttackRangeType.Ranged) return;
        if (fireballPrefab == null || firePointfireball == null || _currentTarget == null) return;
        
        // "애니메이션 두 이벤트 사이 시간"에 맞춰 flyTime을 세팅!
        float fireballFlyTime = 0.33f; // 실제 프레임간 시간(초)로 맞추기
        
        GameObject fireballObj = Instantiate(fireballPrefab, firePointfireball.position, Quaternion.identity);
        Fireball fireballScript = fireballObj.GetComponent<Fireball>();
        if (fireballScript != null)
            fireballScript.SetTarget(_currentTarget.SpawnPosition, fireballFlyTime);
    }

    // 공격/스킬/피격 등 애니메이션
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
    public virtual void PlayDamagedAnim()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("3_Damaged");
    }

    // 방향 전환
    public virtual void LookAt(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position);
        transform.rotation = (dir.x > 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }
    public void ResetRotation() => transform.rotation = _initialRotation;

    // 이동 (부드럽게)
    public async UniTask MoveTo(Vector3 targetPos, float speed = 5f)
    {
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, targetPos);
        float duration = distance / speed;
        float elapsed = 0f;

        var animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.SetBool("1_Move", true);

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }
        transform.position = targetPos;

        if (animator != null) animator.SetBool("1_Move", false);
    }
    public async UniTask MoveToSpawn(float speed = 5f) => await MoveTo(SpawnPosition, speed);

    // 애니메이션 길이 얻기
    public float GetCurrentAttackAnimLength()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator == null) return 0.7f;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains("attack"))
                return clip.length;
        }
        return 0.7f;
    }

    // HP/Heal/기타
    public virtual void TakeDamage(int amount)
    {
        HP = Mathf.Max(0, HP - amount);

        if (healthBarFollower != null)
            healthBarFollower.SetHealth(HP / (float)MaxHP);

        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            if (HP > 0)
            {
                animator.SetTrigger("3_Damaged");
            }
            else
            {
                animator.ResetTrigger("3_Damaged");
                animator.SetTrigger("4_Death");
                // 사망 처리 후 승리 판정 호출!
                DefaultTurnManager.Instance?.CheckVictory();
            }
        }
    }

    public virtual void Heal(int amount)
    {
        HP = Mathf.Min(MaxHP, HP + amount);
        if (healthBarFollower != null)
            healthBarFollower.SetHealth(HP / (float)MaxHP);
    }
}
