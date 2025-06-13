using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class GameInitializer : MonoBehaviour
{
    [Header("Factory & Turn Manager")]
    [SerializeField] private UnitFactory        _factory;
    [SerializeField] private DefaultTurnManager _turnManager;

    [Header("Spawn Point Roots")]
    [SerializeField] private Transform playerSpawnRoot;
    [SerializeField] private Transform enemySpawnRoot;

    [Header("HealthBar Prefab & Canvas")]
    [SerializeField] private HealthBarFollower  healthBarPrefab;
    [SerializeField] private Canvas             uiCanvas;             // 스크린 스페이스 캔버스

    
    void Awake()
    {
        var players = new List<PlayerUnit>();
        foreach (var sp in playerSpawnRoot.GetComponentsInChildren<SpawnPoint>())
        {
            // 1) 유닛 스폰
            var player = _factory
                    .Create(sp.prefab, sp.statId, sp.transform.position, Quaternion.Euler(0,180,0))
                as PlayerUnit;
            if (player == null) continue;
            players.Add(player);

            // 2) 체력바 스폰 & 초기화
            var hb = Instantiate(healthBarPrefab, uiCanvas.transform, worldPositionStays: false);
            hb.Initialize(target: player.transform, offset: new Vector3(0, -0.3f, 0));  
        }

        var enemies = new List<EnemyUnit>();
        foreach (var sp in enemySpawnRoot.GetComponentsInChildren<SpawnPoint>())
        {
            var enemy = _factory
                    .Create(sp.prefab, sp.statId, sp.transform.position, Quaternion.identity)
                as EnemyUnit;
            if (enemy == null) continue;
            enemies.Add(enemy);

            var hb = Instantiate(healthBarPrefab, uiCanvas.transform, worldPositionStays: false);
            hb.Initialize(target: enemy.transform, offset: new Vector3(0, -0.3f, 0));
        }

        _turnManager.InitializeUnits(players, enemies);
    }
}