namespace Game.Input
{
    // 턴 매니저 ↔ 컴뱃 실행기 사이에 주고받을 행동 데이터
    public struct PlayerAction
    {
        public PlayerActionType Type;
        public UnityEngine.MonoBehaviour Target; 
        // IInitializableUnit 대신 MonoBehaviour 로 받으면
        // PlayerUnit / EnemyUnit 모두 할당 가능
    }
}