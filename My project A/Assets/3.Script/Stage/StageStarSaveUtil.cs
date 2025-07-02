using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageStarSaveUtil
{
    // 별 갯수 저장 (기존 기록보다 높을 때만)
    public static void SaveStarCount(string stageId, int stars)
    {
        Debug.Log($"[SaveStarCount] 저장: {stageId}_stars = {stars}");
        PlayerPrefs.SetInt($"{stageId}_stars", stars);
        PlayerPrefs.Save();
    }


    public static int LoadStarCount(string stageId)
    {
        int val = PlayerPrefs.GetInt($"{stageId}_stars", 0);
        Debug.Log($"[LoadStarCount] 로드: {stageId}_stars = {val}");
        return val;
    }
}
