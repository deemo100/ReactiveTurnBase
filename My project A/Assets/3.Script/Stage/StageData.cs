using System.Collections;
using System.Collections.Generic;

// StageData.cs
[System.Serializable]
public class StageData
{
    public string stageId;
    public string stageName;
    public List<int> enemyIds;
    public List<EnemyUnitInfo> Enemies;
    public List<int> rewardIds;
    public string backgroundImage;
    public int requiredPower;
}

[System.Serializable]
public class EnemyUnitInfo
{
    public string prefabName; // ex. "GoblinEnemy1"
    public int statId;
}
