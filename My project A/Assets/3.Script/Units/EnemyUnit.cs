using UnityEngine;

public class EnemyUnit : Unit
{
    private Animator _animator;
    public GroggyBar groggyBar;
    public GroggyBarFollower groggyBarFollower;
    
    void Awake()
    {
        Team = TeamType.Enemy;
        _animator = GetComponentInChildren<Animator>();
    }

    public override void TakeDamage(int amount, int groggy = 0)
    {
        base.TakeDamage(amount, groggy); // 부모에서 승리 판정, HP 등 관리
        // 필요하면 별도 추가 애니메이션 등
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
            Stun(1); // 1턴 스턴
        }
    }

    /// <summary>
    /// 스턴(기절) 상태 부여
    /// </summary>
    public void Stun(int turnCount)
    {
        IsStunned = true;
        stunTurn = turnCount;
        // 필요하면 이펙트/애니메이션 추가
    }

    public void DecreaseStunTurn()
    {
        if (!IsStunned) return;
        stunTurn--;
        if (stunTurn <= 0)
        {
            IsStunned = false;
            Groggy = MaxGroggy; // 그로기 회복(풀)
        }
    }
    
}