using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject prefab;
    public int statId = 1;

    // HP Bar
    public GameObject healthBarPrefab;
    public GameObject groggyBarPrefab;
    public Transform canvasTransform;
    
   //public void SpawnUnit()
   //{
   //    // 1. 유닛 생성
   //    var unitObj = Instantiate(prefab, transform.position, Quaternion.identity);
   //    var unit = unitObj.GetComponent<Unit>(); // PlayerUnit or EnemyUnit
   //
   //    // (InitStat 함수는 필요에 따라 구현)
   //    // unit.InitStat(statId);
   //
   //    // 2. HP Bar 생성 (캔버스 하위)
   //    var hpBarObj = Instantiate(healthBarPrefab, canvasTransform);
   //    var hpFollower = hpBarObj.GetComponent<HealthBarFollower>();
   //    hpFollower.Initialize(unit.transform, new Vector3(0, 2.0f, 0));
   //    unit.healthBar = hpBarObj.GetComponent<HealthBar>();
   //    unit.healthBarFollower = hpFollower;
   //    unit.healthBar.SetHealth(unit.HP / (float)unit.MaxHP);
   //
   //    if (unit is EnemyUnit enemy)
   //    {
   //        Debug.Log("=== [SpawnPoint] EnemyUnit임 ===");
   //        Debug.Log("groggyBarPrefab: " + groggyBarPrefab);
   //
   //        var groggyBarObj = Instantiate(groggyBarPrefab, canvasTransform);
   //        Debug.Log("groggyBarObj: " + groggyBarObj);
   //
   //        var groggyFollower = groggyBarObj.GetComponent<GroggyBarFollower>();
   //        Debug.Log("groggyFollower: " + groggyFollower);
   //
   //        groggyFollower.Initialize(unit.transform, new Vector3(0, 1.7f, 0));
   //        enemy.groggyBar = groggyBarObj.GetComponent<GroggyBar>();
   //        enemy.groggyBarFollower = groggyFollower;
   //        enemy.groggyBar.SetGroggy(enemy.Groggy / (float)enemy.MaxGroggy);
   //    }
   //    else
   //    {
   //        Debug.Log("=== [SpawnPoint] EnemyUnit이 아님 (unit type: " + unit.GetType().Name + ") ===");
   //    }
   //    
   //}
}