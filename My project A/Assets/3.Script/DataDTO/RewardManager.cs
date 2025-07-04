using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RewardManager.cs
public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GiveStageReward(StageData stage, int starCount)
    {
        foreach (var reward in stage.rewards)
        {
            switch (reward.type)
            {
                case "gold":
                    Debug.Log($"골드 획득: {reward.amount}");
                    MoneyManager.Instance.AddGold(reward.amount);
                    break;
                case "gem":
                    string key = $"gemReward_{stage.stageId}";
                    if (reward.amount > 0 && starCount == 3 && PlayerPrefs.GetInt(key, 0) == 0)
                    {
                        Debug.Log($"보석 최초 3성 획득: {reward.amount}");
                        MoneyManager.Instance.AddGem(reward.amount);
                        PlayerPrefs.SetInt(key, 1);
                        PlayerPrefs.Save();
                    }
                    break;
            }
        }
    }
}

