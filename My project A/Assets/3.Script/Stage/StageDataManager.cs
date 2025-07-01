// StageDataManager.cs
using UnityEngine;
using System.Collections.Generic;

public class StageDataManager : MonoBehaviour
{
    public static StageDataManager Instance { get; private set; }
    public List<StageData> Stages { get; private set; } = new List<StageData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadStageData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadStageData()
    {
        TextAsset ta = Resources.Load<TextAsset>("StageData");
        if (ta == null)
        {
            Debug.LogError("StageData.json 파일이 Resources 폴더에 없음!");
            return;
        }
        Stages = JsonUtilityWrapper.FromJsonList<StageData>(ta.text);
    }
    
    public StageData StagesFindById(string id)
    {
        return Stages.Find(s => s.stageId == id);
    }
    
}