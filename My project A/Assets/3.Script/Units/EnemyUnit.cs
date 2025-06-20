using UnityEngine;

public class EnemyUnit : Unit
{
    private Animator _animator;

    void Awake()
    {
        Team = TeamType.Enemy;
        _animator = GetComponent<Animator>();
    }

    public override void TakeDamage(int amount)
    {
        Debug.Log($"[EnemyUnit] TakeDamage 호출됨, {UnitName}");
        base.TakeDamage(amount);

        if (_animator != null)
        {
            if (HP > 0)
            {
                Debug.Log("[EnemyUnit] Damaged 트리거 실행!");
                _animator.SetTrigger("Damaged");
            }
            else
            {
                Debug.Log("[EnemyUnit] Death 트리거 실행!");
                _animator.SetTrigger("Death");
            }
        }
    }

    public void PlayAttackAnim()
    {
        if (_animator != null)
        {
            Debug.Log("[EnemyUnit] Attack 트리거 실행!");
            _animator.SetTrigger("Attack");
        }
    }
}