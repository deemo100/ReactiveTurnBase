using System.Collections.Generic;

[System.Serializable]
public class PlayerUnitSaveData
{
    public int statId;
    public int level; // 캐릭터 레벨
    public List<int> equipmentList; // 장비 ID 리스트 (or 장비 구조체로 확장)
    public List<int> skillLevels;   // 각 스킬의 레벨 (예: 0~n, 순서대로)
  
}