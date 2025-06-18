using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("이 포인트에서 소환할 프리팹")]
    public GameObject prefab;

    [Tooltip("이 프리팹으로 InitStat 에 넘길 statId")]
    public int statId = 1;

    // 🟡 [추가] HP Bar 프리팹 & UI Canvas Transform
    public GameObject healthBarPrefab;   // Inspector에서 드래그
    public Transform canvasTransform;    // Inspector에서 드래그

    // 예시: 유닛 소환 함수
    public void SpawnUnit()
    {
        // 1. 유닛 생성
        var unitObj = Instantiate(prefab, transform.position, Quaternion.identity);
        var unit = unitObj.GetComponent<Unit>(); // PlayerUnit, EnemyUnit 등

        // (InitStat 함수는 필요에 따라 구현)
        // unit.InitStat(statId);

        // 2. HP Bar 생성(캔버스 하위)
        var hpBarObj = Instantiate(healthBarPrefab, canvasTransform);
        var follower = hpBarObj.GetComponent<HealthBarFollower>();
        follower.Initialize(unit.transform, new Vector3(0, 2.0f, 0));

        // 3. 유닛에 HP Bar 연결
        unit.healthBar = hpBarObj.GetComponent<HealthBar>();
        unit.healthBarFollower = follower;

        // 4. (선택) 체력 UI 초기화
        unit.healthBar.SetHealth(unit.HP / (float)unit.MaxHP);
    }
}