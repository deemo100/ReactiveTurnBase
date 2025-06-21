using UnityEngine;

public class EnemyUnit : Unit
{
    private Animator _animator;

    void Awake()
    {
        Team = TeamType.Enemy;
        _animator = GetComponentInChildren<Animator>();
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount); // 부모에서 승리 판정, HP 등 관리

        // 필요하면 별도 추가 애니메이션 등만 여기에!
        // ex: 사운드, 특수 이펙트 등
    }

    public void PlayAttackAnim()
    {
        if (_animator != null)
        {
            Debug.Log("[EnemyUnit] Attack 트리거 실행!");
            _animator.SetTrigger("2_Attack");
        }
    }
}