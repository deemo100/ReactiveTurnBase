[System.Serializable]
public class PlayerSelectData
{
    public string prefabName; // Resources/PlayerPrefabs/폴더 하위 프리팹명 (ex: "Warrior", "Priest" 등)
    public int statId;        // UnitStatTable의 id
    // 장비, 스킬, 레벨 등도 추가 가능
}