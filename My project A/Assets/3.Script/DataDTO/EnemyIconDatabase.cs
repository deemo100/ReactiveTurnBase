using UnityEngine;

[System.Serializable]
public class EnemyIconEntry
{
    public int id;        // 예: 고블린=1, 고블린전사=2
    public Sprite icon;   // 매칭할 이미지
}

[CreateAssetMenu(menuName = "DB/EnemyIconDatabase")]
public class EnemyIconDatabase : ScriptableObject
{
    public EnemyIconEntry[] icons;

    // id로 Sprite 찾기
    public Sprite GetIconById(int id)
    {
        foreach (var entry in icons)
            if (entry.id == id) return entry.icon;
        return null;
    }
}