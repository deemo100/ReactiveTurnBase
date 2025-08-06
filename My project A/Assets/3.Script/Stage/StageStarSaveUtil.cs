using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageStarSaveUtil
{
    // 별 갯수 저장 (기존 기록보다 높을 때만)
    public static void SaveStarCount(string stageId, int stars)
    {
        int prev = LoadStarCount(stageId);
        if (stars > prev)   // 기존 기록보다 높을 때만 저장
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
        int val = PlayerPrefs.GetInt($"{stageId}_stars", 0);
        return val;
    }
}
