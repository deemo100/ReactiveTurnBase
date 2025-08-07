using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageStarSaveUtil
{
    public static void ResetAllStageData()
    {
        string[] stageIds = { "stage_1_1", "stage_1_2", "stage_1_3" };
        foreach (var stageId in stageIds)
        {
            PlayerPrefs.DeleteKey($"{stageId}_stars");         // ← 저장과 동일하게!
            PlayerPrefs.DeleteKey($"gemReward_{stageId}");     // (이건 유지)
        }
        PlayerPrefs.Save();
    }

    // 별 갯수 저장 (기존 기록보다 높을 때만)
    public static void SaveStarCount(string stageId, int stars)
    {
        int prev = LoadStarCount(stageId);
        if (stars > prev)
        {
            Debug.Log($"[SaveStarCount] 별 갱신! {stageId}_stars : {prev} → {stars}");
            PlayerPrefs.SetInt($"{stageId}_stars", stars);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log($"[SaveStarCount] 기존 별({prev}) >= 신규 별({stars}), 저장하지 않음");
        }
    }

    public static int LoadStarCount(string stageId)
    {
        return PlayerPrefs.GetInt($"{stageId}_stars", 0);
    }
}