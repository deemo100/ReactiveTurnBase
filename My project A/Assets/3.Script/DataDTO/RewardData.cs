[System.Serializable]
public class RewardData
{
    public string type;   // "gold", "gem", "item"
    public int amount;
    public int itemId;    // 아이템일 때만 사용
}