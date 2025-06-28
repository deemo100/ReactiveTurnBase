using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Input;
using Cysharp.Threading.Tasks;

public enum TeamType { Player, Enemy }
public enum AttackRangeType { Melee, Ranged }

public class Unit : MonoBehaviour
{
    protected List<Unit> _currentTargets = new List<Unit>();

    public TeamType Team { get; set; }
    public AttackRangeType AttackType = AttackRangeType.Melee;

    [Header("원거리 유닛만 설정")]
    public GameObject arrowPrefab;
    public GameObject fireballPrefab;
    public Transform firePoint;
    public Transform firePointfireball;

    [Header("임펙트 이펙트(예: 불기둥)")]
    public GameObject aoeImpactPrefab;

    public Vector3 SpawnPosition { get; private set; }
    public float MoveSpeed = 10f;
    public int Id { get; protected set; }
    public string UnitName { get; protected set; }
    public int MaxHP { get; protected set; }
    public int HP { get; protected set; }
    public int ATK { get; set; }
    public int DEF { get; protected set; }
    public int MaxGroggy { get; protected set; }
    public int Groggy { get; protected set; }
    public bool IsStunned { get; protected set; } = false;
    protected int stunTurn = 0;
    
    public bool IsDead => HP <= 0;
    public HealthBar healthBar;
    public HealthBarFollower healthBarFollower;
    public GroggyBar groggyBar;
    public GroggyBarFollower groggyBarFollower;
    
    public SkillData SkillData { get; protected set; }

    
    public bool IsBusy { get; private set; } = false; // ★추가
    public void SetBusy(bool busy) => IsBusy = busy; // ★추가
    
    Quaternion _initialRotation;

    public int NormalAttackPercent = 100; // 인스펙터에서 기본값 100
    public int NormalAttackGroggy;
    
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
        MaxGroggy = stat.MaxGroggy;
        Groggy = stat.MaxGroggy;
    }

    // 타겟 지정
    public void SetAttackTarget(Unit t)
    {
        _currentTargets.Clear();
        if (t != null && !t.IsDead)
            _currentTargets.Add(t);
    }
    public void SetAttackTargets(List<Unit> targets)
    {
        _currentTargets = targets.Where(x => x != null && !x.IsDead).ToList();
    }

    // 일반 공격 임팩트(애니메이션 이벤트)
    public virtual void OnAttackImpact()
    {
        if (_currentTargets == null || _currentTargets.Count == 0) return;
        foreach (var unit in _currentTargets)
        {
            if (unit == null || unit.IsDead) continue;
            int damage = Mathf.Max(0, Mathf.RoundToInt(ATK * (NormalAttackPercent / 100f)) - unit.DEF);
            int groggy = NormalAttackGroggy;
            Debug.Log($"[공격] GroggyDamage: {groggy} (ATK:{ATK}, Percent:{NormalAttackPercent})");
            unit.TakeDamage(damage, groggy);
        }
        _currentTargets.Clear();
    }

    // --- 스킬 공격 (공격력 x 계수) ---
    public virtual void OnSkillImpact()
    {
        if (_currentTargets == null || _currentTargets.Count == 0 || SkillData == null) return;

        foreach (var unit in _currentTargets)
        {
            if (unit == null || unit.IsDead) continue;

            if (SkillData.EffectType == SkillEffectType.Damage)
            {
                int damage = Mathf.Max(0, Mathf.RoundToInt(ATK * (SkillData.Power / 100f)) - unit.DEF);
                int groggy = SkillData.GroggyDamage;
                Debug.Log($"[스킬] GroggyDamage: {groggy} (Power:{SkillData.Power})");
                unit.TakeDamage(damage, groggy); // 🔹 수정된 부분
            }
            else if (SkillData.EffectType == SkillEffectType.Heal)
            {
                unit.Heal(SkillData.Power);
            }
        }

        _currentTargets.Clear();
    }

    // 힐 전용 이벤트(필요시)
    public void OnHealImpact()
    {
        if (_currentTargets == null || _currentTargets.Count == 0 || SkillData == null) return;
        foreach (var unit in _currentTargets)
        {
            if (unit == null || unit.IsDead) continue;
            if (unit.HP < unit.MaxHP)
                unit.Heal(SkillData.Power);
        }
        _currentTargets.Clear();
    }

    // 광역 임펙트(불기둥 등) - EnemyAll 스킬에서 애니메이션 이벤트로 호출
    public void OnAOEImpact()
    {
        var enemies = FindObjectsOfType<EnemyUnit>().Where(e => !e.IsDead).ToList();
        if (enemies.Count == 0 || aoeImpactPrefab == null) return;
        Vector3 center = Vector3.zero;
        foreach (var e in enemies)
            center += e.transform.position;
        center /= enemies.Count;
        center.y += 2.5f; // Y 오프셋
        Instantiate(aoeImpactPrefab, center, Quaternion.identity);
    }

    // 화살/파이어볼 발사 (투사체 애니메이션 이벤트)
    public void FireArrowFX()
    {
        if (AttackType != AttackRangeType.Ranged) return;
        if (arrowPrefab == null || firePoint == null) return;
        if (_currentTargets == null || _currentTargets.Count == 0) return;

        float arrowFlyTime = 0.33f;
        var targetUnit = _currentTargets[0];
        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrowScript = arrowObj.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.SetTarget(targetUnit.SpawnPosition, arrowFlyTime);
    }

    public void FireFireballFX()
    {
        if (AttackType != AttackRangeType.Ranged) return;
        if (fireballPrefab == null || firePointfireball == null) return;
        if (_currentTargets == null || _currentTargets.Count == 0) return;

        float fireballFlyTime = 0.33f;
        var targetUnit = _currentTargets[0];
        GameObject fireballObj = Instantiate(fireballPrefab, firePointfireball.position, Quaternion.identity);
        Fireball fireballScript = fireballObj.GetComponent<Fireball>();
        if (fireballScript != null)
            fireballScript.SetTarget(targetUnit.SpawnPosition, fireballFlyTime);
    }

    // 애니메이션 재생 함수
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
        if (IsDead) return;
        if (IsStunned) return; // 기절 중이면 데미지 애니메이션 생략!
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

    // 이동
    public async UniTask MoveTo(Vector3 targetPos, float speed = 5f)
    {
        if (this == null) return; // ★추가
        Vector3 start = transform.position;
        float distance = Vector3.Distance(start, targetPos);
        float duration = distance / speed;
        float elapsed = 0f;

        var animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.SetBool("1_Move", true);

        while (elapsed < duration)
        {
            if (this == null) return; // ★추가
            transform.position = Vector3.Lerp(start, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            await UniTask.Yield();
        }
        transform.position = targetPos;

        if (animator != null) animator.SetBool("1_Move", false);
    }
    public async UniTask MoveToSpawn(float speed = 5f) => await MoveTo(SpawnPosition, speed);

    // 애니메이션 길이
    public float GetCurrentAttackAnimLength()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator == null) return 0.3f;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains("attack"))
                return clip.length;
        }
        return 0.3f;
    }

    // 데미지/회복 처리
    public virtual void TakeDamage(int amount, int groggy = 0)
    {
        if (IsDead) return;
        HP = Mathf.Max(0, HP - amount);
        if (healthBarFollower != null)
            healthBarFollower.SetHealth(HP / (float)MaxHP);

        // 그로기 적용
        if (this is EnemyUnit enemy && groggy > 0)
            enemy.TakeGroggy(groggy);

        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            // 스턴 중엔 데미지 애니메이션 금지!
            if (!IsStunned)
            {
                if (HP > 0)
                    animator.SetTrigger("3_Damaged");
                else
                {
                    animator.ResetTrigger("3_Damaged");
                    animator.SetTrigger("4_Death");
                    DefaultTurnManager.Instance?.CheckVictory();
                    
                    if (healthBarFollower != null)
                        healthBarFollower.gameObject.SetActive(false);
                    if (groggyBarFollower != null)
                        groggyBarFollower.gameObject.SetActive(false); // 🟢 추가됨
                }
            }
            else
            {
                // 스턴 중에도 죽었으면, 데스 애니메이션만 실행
                if (HP <= 0)
                {
                    animator.ResetTrigger("3_Damaged");
                    animator.SetTrigger("4_Death");
                    DefaultTurnManager.Instance?.CheckVictory();
                    
                    if(healthBarFollower != null)
                        healthBarFollower.gameObject.SetActive(false);
                    if(groggyBarFollower != null)
                        groggyBarFollower.gameObject.SetActive(false);
                }
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
