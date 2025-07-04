using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class StageData
{
    public string stageId;
    public string stageName;
    public List<int> enemyIds;
    public List<EnemyUnitInfo> Enemies;
    public List<RewardData> rewards;      // ⭐ 이걸로 보상 리스트 통합!
    public string backgroundImage;
    public int requiredPower;
    public int requiredEnergy;
}

[System.Serializable]
public class EnemyUnitInfo
{
    public string prefabName; // ex. "GoblinEnemy1"
    public int statId;
}

[System.Serializable]
public class StageReward
{
    public int gold;
    public int gem;
    public List<int> items;
}
