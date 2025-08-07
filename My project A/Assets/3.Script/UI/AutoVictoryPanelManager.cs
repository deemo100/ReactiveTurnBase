using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoVictoryPanelManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject rootPanel; // 내부 패널만 껐다 켜기
    public Image[] rewardSlots;
    public Sprite goldIcon, gemIcon, itemIcon;
    public Button exitButton;

    private StageData currentStageData;

    void Awake()
    {
        rootPanel.SetActive(false);
        if (exitButton != null)
            exitButton.onClick.AddListener(() => rootPanel.SetActive(false));
    }

    /// <summary>
    /// 오토 클리어 보상 지급 + 패널 띄우기
    /// </summary>
    public void ShowAutoVictory(StageData stageData)
    {
        currentStageData = stageData;
        rootPanel.SetActive(true);
        
        // 2. 보상 지급 (보석은 1회성)
        GiveClearReward(stageData);

        // 3. 보상 UI 표시
        ShowClearRewards(stageData);
    }

    private void GiveClearReward(StageData stage)
    {
        foreach (var reward in stage.rewards)
        {
            if (reward.type == "gem")
            {
                string gemKey = $"gemReward_{stage.stageId}";
                bool gemReceived = PlayerPrefs.GetInt(gemKey, 0) == 1;
                if (gemReceived) continue;
                int star = StageStarSaveUtil.LoadStarCount(stage.stageId);
                if (star < 3) continue;
                MoneyManager.Instance.AddGem(reward.amount);
                PlayerPrefs.SetInt(gemKey, 1);
            }
            else if (reward.type == "gold")
            {
                MoneyManager.Instance.AddGold(reward.amount);
            }
            // 기타 아이템 등도 필요 시 추가
        }
        StageStarSaveUtil.SaveStarCount(stage.stageId, 3);
    }

    public void ShowClearRewards(StageData stage)
    {
        for (int i = 0; i < rewardSlots.Length; i++)
            rewardSlots[i].gameObject.SetActive(false);

        int slotIdx = 0;
        foreach (var reward in stage.rewards)
        {
            if (reward.type == "gem")
            {
                string gemKey = $"gemReward_{stage.stageId}";
                bool gemReceived = PlayerPrefs.GetInt(gemKey, 0) == 1;
                if (gemReceived) continue;
            }

            if (slotIdx >= rewardSlots.Length) break;

            rewardSlots[slotIdx].gameObject.SetActive(true);

            switch (reward.type)
            {
                case "gold":
                    rewardSlots[slotIdx].sprite = goldIcon;
                    break;
                case "gem":
                    rewardSlots[slotIdx].sprite = gemIcon;
                    break;
                default:
                    rewardSlots[slotIdx].sprite = itemIcon;
                    break;
            }
            slotIdx++;
        }
    }

    /// <summary>
    /// 외부에서 직접 종료시 호출
    /// </summary>
    public void Hide()
    {
        rootPanel.SetActive(false);
    }
}
