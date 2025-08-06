using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoVictoryPanelManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject rootPanel;        // 오토 결과 전체 패널
    public Image[] rewardSlots;         // 보상 아이콘 이미지 배열
    public Sprite goldIcon, gemIcon, itemIcon;
    public Button exitButton;           // 확인/닫기 버튼

    // 데이터
    private StageData currentStageData;

    // 외부에서 호출
    public void ShowAutoVictory(StageData stageData)
    {
        currentStageData = stageData;
        rootPanel.SetActive(true);

        // 보상 표시
        ShowAutoClearRewards(stageData);

        // 버튼 리스너 초기화
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => rootPanel.SetActive(false));
    }

    // 보상 아이콘만 표시
    private void ShowAutoClearRewards(StageData stage)
    {
        for (int i = 0; i < rewardSlots.Length; i++)
            rewardSlots[i].gameObject.SetActive(false);

        int slotIdx = 0;
        foreach (var reward in stage.rewards)
        {
            // 예: gem 보상은 이미 받았으면 제외
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
                case "gold": rewardSlots[slotIdx].sprite = goldIcon; break;
                case "gem": rewardSlots[slotIdx].sprite = gemIcon; break;
                default: rewardSlots[slotIdx].sprite = itemIcon; break;
            }
            slotIdx++;
        }
    }
}