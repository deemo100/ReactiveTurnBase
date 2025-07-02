using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageStarSaveUtil
{
    // 별 갯수 저장 (기존 기록보다 높을 때만)
    public static void SaveStarCount(string stageId, int stars)
    {
        int prev = PlayerPrefs.GetInt($"{stageId}_stars", 0);
        if (stars > prev)
        {
            PlayerPrefs.SetInt($"{stageId}_stars", stars);
            PlayerPrefs.Save();
        }
    }

    // 별 갯수 불러오기
    public static int LoadStarCount(string stageId)
    {
        return PlayerPrefs.GetInt($"{stageId}_stars", 0);
    }
}
