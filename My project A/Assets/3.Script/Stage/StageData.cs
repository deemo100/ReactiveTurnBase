using System.Collections;
using System.Collections.Generic;

// StageData.cs
[System.Serializable]
public class StageData
{
    public string stageId;
    public string stageName;
    public List<int> enemyIds;
    public List<int> rewardIds;
    public string backgroundImage;
    public int requiredPower;
}
