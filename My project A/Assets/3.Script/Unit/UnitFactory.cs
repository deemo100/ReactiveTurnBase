// Scripts/Factory/UnitFactory.cs
using UnityEngine;

/// <summary>
/// DataManager에서 로드된 UnitStat/SkillData를 바탕으로 플레이어와 적 유닛을 생성하고 초기화합니다.
/// </summary>
public class UnitFactory : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    /// <summary>
    /// statId에 해당하는 PlayerUnit 인스턴스를 생성하고 초기화한 후 반환합니다.
    /// </summary>
    public PlayerUnit CreatePlayer(int statId, Vector3 position)
    {
        var go = Instantiate(playerPrefab, position, Quaternion.identity);
        var unit = go.GetComponent<PlayerUnit>();
        var stat = DataManager.Instance.UnitStatTable[statId];
        unit.Init(stat);
        // 단일 스킬 시스템 가정: statId == skillId
        unit.SkillData = DataManager.Instance.SkillTable[statId];
        return unit;
    }

    /// <summary>
    /// statId에 해당하는 EnemyUnit 인스턴스를 생성하고 초기화한 후 반환합니다.
    /// </summary>
    public EnemyUnit CreateEnemy(int statId, Vector3 position)
    {
        var go = Instantiate(enemyPrefab, position, Quaternion.identity);
        var unit = go.GetComponent<EnemyUnit>();
        var stat = DataManager.Instance.UnitStatTable[statId];
        unit.Init(stat);
        unit.SkillData = DataManager.Instance.SkillTable[statId];
        return unit;
    }
}