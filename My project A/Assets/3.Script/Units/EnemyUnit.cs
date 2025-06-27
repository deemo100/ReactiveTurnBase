using UnityEngine;

public class EnemyUnit : Unit
{
    private Animator _animator;
    public GroggyBar groggyBar;
    public GroggyBarFollower groggyBarFollower;
    private bool needGroggyRecover = false;
    
    void Awake()
    {
        Team = TeamType.Enemy;
        _animator = GetComponentInChildren<Animator>();
    }

    public override void TakeDamage(int amount, int groggy = 0)
    {
        base.TakeDamage(amount, groggy); // 부모에서 HP 감소 및 승리 판정

        if (HP <= 0)  // 죽었으면 직접 그로기바도 끄기
        {
            if (groggyBarFollower != null)
            {
                Debug.Log($"[EnemyUnit] {gameObject.name} 그로기바 숨김 처리");
                groggyBarFollower.gameObject.SetActive(false);
            }
        }
    }
    
    
    public void PlayAttackAnim()
    {
        if (_animator != null)
        {
            Debug.Log("[EnemyUnit] Attack 트리거 실행!");
            _animator.SetTrigger("2_Attack");
        }
    }
    
    /// <summary>
    /// 그로기 피해 처리
    /// </summary>
    public void TakeGroggy(int amount)
    {
        if (IsStunned) return;
        int prev = Groggy;
        Groggy = Mathf.Max(0, Groggy - amount);

        Debug.Log($"[Groggy] {prev} → {Groggy} / Max:{MaxGroggy} (-{amount}, 비율={Groggy/(float)MaxGroggy:F2})");

        if (groggyBarFollower != null)
            groggyBarFollower.SetGroggy(Groggy / (float)MaxGroggy);

        if (Groggy == 0)
        {
            Stun(2); // 2턴 스턴
        }
    }

    public void Stun(int turnCount)
    {
        // ⭐ 죽었으면 스턴 애니메이션 생략
        if (HP <= 0 || IsDead)
            return;

        IsStunned = true;
        stunTurn = turnCount;

        if (_animator != null)
        {
            _animator.SetBool("IsStunned", true);
            _animator.SetBool("8_Stun", true);
            _animator.ResetTrigger("3_Damaged");
            _animator.SetTrigger("6_Sit");
        }
    }

    public override void PlayDamagedAnim()
    {
        if (IsDead) return;
        if (IsStunned) return; // ⭐ 스턴 중이면 데미지 애니메이션 무시!
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("3_Damaged");
    }
    
    public void DecreaseStunTurn()
    {
        if (!IsStunned) return;
        stunTurn--;

        // ⭐스턴이 아직 남아 있으면 아무것도 하지 않음(애니메이션 그대로)
        if (stunTurn > 0)
            return;

        // ⭐여기서만 스턴 완전히 해제! (스턴바 회복, 애니메이션 종료)
        IsStunned = false;
        Groggy = MaxGroggy;
        if (_animator != null)
        {
            _animator.SetBool("IsStunned", false);
            _animator.SetBool("8_Stun", false);
        }
        if (groggyBarFollower != null)
            groggyBarFollower.SetGroggy(1f); // 바도 풀로
    }
    
    // EnemyPhase에서 행동 전 호출
    public void RecoverGroggyIfNeeded()
    {
        if (needGroggyRecover)
        {
            Groggy = MaxGroggy;
            if (groggyBarFollower != null)
                groggyBarFollower.SetGroggy(1f);
            needGroggyRecover = false;
        }
    }
}