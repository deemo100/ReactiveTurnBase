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
        if (stage == null)
        {
            Debug.LogError("GiveStageReward: stage 데이터가 null!");
            return;
        }

        foreach (var reward in stage.rewards)
        {
            switch (reward.type)
            {
                case "gold":
                    MoneyManager.Instance.AddGold(reward.amount);
                    Debug.Log($"[Reward] gold: {reward.amount}");
                    break;
                case "gem":
                    string key = $"gemReward_{stage.stageId}";
                    if (reward.amount > 0 && starCount == 3 && PlayerPrefs.GetInt(key, 0) == 0)
                    {
                        MoneyManager.Instance.AddGem(reward.amount);
                        PlayerPrefs.SetInt(key, 1);
                        PlayerPrefs.Save();
                        Debug.Log($"[Reward] gem: {reward.amount}");
                    }
                    break;
            }
        }
    }
}

