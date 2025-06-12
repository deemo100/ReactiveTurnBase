using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class GameInitializer : MonoBehaviour
{
    [SerializeField] UnitFactory        _factory;
    [SerializeField] DefaultTurnManager _turnManager;

    [Header("스폰 지점 Root")]
    [SerializeField] Transform playerSpawnRoot;
    [SerializeField] Transform enemySpawnRoot;

    void Awake()
    {
        // 플레이어
        var players = new List<PlayerUnit>();
        foreach (var sp in playerSpawnRoot.GetComponentsInChildren<SpawnPoint>())
        {
            var unit = _factory.Create(sp.prefab, sp.statId, sp.transform.position)
                as PlayerUnit;
            if (unit != null) players.Add(unit);
        }

        // 적
        var enemies = new List<EnemyUnit>();
        foreach (var sp in enemySpawnRoot.GetComponentsInChildren<SpawnPoint>())
        {
            var unit = _factory.Create(sp.prefab, sp.statId, sp.transform.position)
                as EnemyUnit;
            if (unit != null) enemies.Add(unit);
        }

        _turnManager.InitializeUnits(players, enemies);
    }
}