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
    [SerializeField] private Canvas             uiCanvas;

    void Awake()
    {
        var players = new List<PlayerUnit>();
        foreach (var sp in playerSpawnRoot.GetComponentsInChildren<SpawnPoint>())
        {
            // 1. 유닛 생성
            var player = _factory
                    .Create(sp.prefab, sp.statId, sp.transform.position, TeamType.Player, Quaternion.Euler(0,180,0))
                as PlayerUnit;
            if (player == null) continue;
            players.Add(player);

            // 2. 체력바 프리팹 생성 및 연결
            var hb = Instantiate(healthBarPrefab, uiCanvas.transform, false);
            hb.Initialize(player.transform, new Vector3(0, -0.3f, 0));
            player.healthBarFollower = hb;
            player.healthBar = hb.GetComponent<HealthBar>();
            // HP UI 초기화
            player.healthBarFollower.SetHealth(player.HP / (float)player.MaxHP);
        }

        var enemies = new List<EnemyUnit>();
        foreach (var sp in enemySpawnRoot.GetComponentsInChildren<SpawnPoint>())
        {
            var enemy = _factory
                    .Create(sp.prefab, sp.statId, sp.transform.position, TeamType.Enemy, Quaternion.identity)
                as EnemyUnit;
            if (enemy == null) continue;
            enemies.Add(enemy);

            var hb = Instantiate(healthBarPrefab, uiCanvas.transform, false);
            hb.Initialize(enemy.transform, new Vector3(0, -0.3f, 0));
            enemy.healthBarFollower = hb;
            enemy.healthBar = hb.GetComponent<HealthBar>();
            enemy.healthBarFollower.SetHealth(enemy.HP / (float)enemy.MaxHP);
        }

        _turnManager.InitializeUnits(players, enemies);
    }
}