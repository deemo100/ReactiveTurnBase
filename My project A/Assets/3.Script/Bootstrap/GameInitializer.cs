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
    [SerializeField] private GroggyBarFollower groggyBarPrefab;
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

            // HP Bar 생성
            var hb = Instantiate(healthBarPrefab, uiCanvas.transform, false);
            hb.gameObject.SetActive(true); // ← 무조건 활성화 (보험)
            hb.Initialize(enemy.transform, new Vector3(0, -0.3f, 0));
            enemy.healthBarFollower = hb;
            enemy.healthBar = hb.GetComponent<HealthBar>();

            // ⭐ HP Bar NaN/0 방지
            float maxHp = Mathf.Max(1, enemy.MaxHP); // 0 방지
            float hpNormalized = (float)enemy.HP / maxHp;
            enemy.healthBarFollower.SetHealth(hpNormalized);

            // 🟣 Groggy Bar 생성
            var gb = Instantiate(groggyBarPrefab, uiCanvas.transform, false);
            gb.gameObject.SetActive(true); // ← 무조건 활성화 (보험)
            gb.Initialize(enemy.transform, new Vector3(0, -0.55f, 0)); // HP Bar보다 아래쪽에 붙이기
            enemy.groggyBarFollower = gb;
            enemy.groggyBar = gb.GetComponent<GroggyBar>();

            // ⭐ Groggy Bar NaN/0 방지
            float maxGroggy = Mathf.Max(1, enemy.MaxGroggy);
            float groggyNormalized = (float)enemy.Groggy / maxGroggy;
            enemy.groggyBarFollower.SetGroggy(groggyNormalized);
        }
        _turnManager.InitializeUnits(players, enemies);
        
    }
}