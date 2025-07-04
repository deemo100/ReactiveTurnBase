using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }
    
    public List<PlayerSelectData> selectedPlayers = new List<PlayerSelectData>();
   
    // 현재 선택된 스테이지 ID (메뉴 등에서 할당)
    public string currentStageId;
    public StageData currentStageData;   // ⭐ 현재 스테이지 데이터(추가)
    public int lastClearStarCount = 0;
    
    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }
}

[System.Serializable]
public class PlayerUnitInfo
{
    public string prefabName; // 예: "player1"
    public int statId;        // ex. 1, 2, 3...
}