using UnityEngine;
using System.Collections.Generic;

public class GameInitializer : MonoBehaviour
{
    [Header("필수 참조")]
    [SerializeField] private UnitFactory        _factory;
    [SerializeField] private DefaultTurnManager _turnManager;
    [SerializeField] private Transform          playerSpawnRoot;
    [SerializeField] private Transform          enemySpawnRoot;
    [SerializeField] private HealthBarFollower  healthBarPrefab;
    [SerializeField] private GroggyBarFollower  groggyBarPrefab;
    [SerializeField] private Canvas             uiCanvas;

    void Awake()
    {
        // 1. 세션, 스테이지 정보 받아오기
        var session = GameSession.Instance;
        var stageData = StageDataManager.Instance.StagesFindById(session.currentStageId);

        Debug.Log($"[GameInit] 선택된 플레이어 수: {session.selectedPlayers?.Count ?? -1}");
        Debug.Log($"[GameInit] 스테이지 적 유닛 수: {stageData?.Enemies?.Count ?? -1}");
        
        // 2. 플레이어 생성
        var players = new List<PlayerUnit>();
        var playerSpawnPoints = playerSpawnRoot.GetComponentsInChildren<SpawnPoint>();
        for (int i = 0; i < session.selectedPlayers.Count && i < playerSpawnPoints.Length; i++)
        {
            var data = session.selectedPlayers[i];
            var sp = playerSpawnPoints[i];
            var prefab = Resources.Load<GameObject>($"PlayerPrefabs/{data.prefabName}");
            if (prefab == null) {
                Debug.LogError($"[GameInit] Player 프리팹 로드 실패! {data.prefabName}");
                continue;
            }
            var player = _factory.Create(prefab, data.statId, sp.transform.position, TeamType.Player, Quaternion.Euler(0, 180, 0)) as PlayerUnit;
            if (player == null) {
                Debug.LogError($"[GameInit] Player 유닛 생성 실패! {data.prefabName}, statId={data.statId}");
                continue;
            }
            players.Add(player); // ← 이게 실제로 들어가는지
            
            var hb = Instantiate(healthBarPrefab, uiCanvas.transform, false);
            hb.Initialize(player.transform, new Vector3(0, -0.3f, 0));
            player.healthBarFollower = hb;
            player.healthBar = hb.GetComponent<HealthBar>();
            player.healthBarFollower.SetHealth(player.HP / (float)player.MaxHP);
        }

        // 3. 적 생성 (스테이지 데이터 기반)
        var enemies = new List<EnemyUnit>();
        var enemySpawnPoints = enemySpawnRoot.GetComponentsInChildren<SpawnPoint>();
        for (int i = 0; i < stageData.Enemies.Count && i < enemySpawnPoints.Length; i++)
        {
            var data = stageData.Enemies[i];
            var sp = enemySpawnPoints[i];
            var prefab = Resources.Load<GameObject>($"EnemyPrefabs/{data.prefabName}");
            if (prefab == null) {
                Debug.LogError($"[GameInit] Enemy 프리팹 로드 실패! {data.prefabName}");
                continue;
            }

            var enemy = _factory.Create(
                prefab,
                data.statId,
                sp.transform.position,
                TeamType.Enemy,
                Quaternion.identity
            ) as EnemyUnit;
            if (enemy == null) {
                Debug.LogError($"[GameInit] Enemy 유닛 생성 실패! {data.prefabName}, statId={data.statId}");
                continue;
            }
            enemies.Add(enemy); // ← 이게 실제로 들어가는지
          
            var ehb = Instantiate(healthBarPrefab, uiCanvas.transform, false);
            ehb.Initialize(enemy.transform, new Vector3(0, -0.3f, 0));
            enemy.healthBarFollower = ehb;
            enemy.healthBar = ehb.GetComponent<HealthBar>();
            enemy.healthBarFollower.SetHealth(enemy.HP / (float)enemy.MaxHP);
            
            var gb = Instantiate(groggyBarPrefab, uiCanvas.transform, false);
            gb.Initialize(enemy.transform, new Vector3(0, -0.55f, 0)); // HP바보다 아래
            enemy.groggyBarFollower = gb;
            enemy.groggyBar = gb.GetComponent<GroggyBar>();
            enemy.groggyBarFollower.SetGroggy(enemy.Groggy / (float)Mathf.Max(1, enemy.MaxGroggy));
            
        }
        _turnManager.InitializeUnits(players, enemies);
    }
}
