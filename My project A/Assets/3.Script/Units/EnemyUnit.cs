using UnityEngine;

public class EnemyUnit : Unit
{
    void Awake()
    {
        Team = TeamType.Enemy; // 반드시 명확히 할당
    }
    // 추가로 적 AI, 특화 행동 등 구현
}