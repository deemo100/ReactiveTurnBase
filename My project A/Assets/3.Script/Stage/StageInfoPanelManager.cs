using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StageInfoPanelManager : MonoBehaviour
{
    [Header("UI 요소")]
    public GameObject rootPanel;
    public TMP_Text titleText;
    public Transform enemyListParent;
    public Transform rewardListParent;
    public Button enterButton;
    public Button closeButton;

    // (적/보상 아이콘용 프리팹)
    public GameObject enemyIconPrefab;
    
    [Header("보상 프리팹")]
    public GameObject moneyIcon;
    public GameObject gemIcon;
    public GameObject rewardIconPrefab;

    public EnemyIconDatabase enemyIconDB; 
    
    // ⭐⭐ [추가] 여러 스테이지별 별 그룹 (score1, score2 등)
    [Header("별 UI 그룹")]
    public GameObject[] scoreGroups; 

    // 스테이지 데이터 참조
    private StageData currentStageData;
    public TMP_Text costText;
    
    void Awake()
    {
        rootPanel.SetActive(false); // 기본 비활성화
        closeButton.onClick.AddListener(() => rootPanel.SetActive(false));
    }

    // 외부에서 호출 (예: 스테이지 버튼 클릭 시)
    public void Show(StageData stage, StageButton btn)
    {
        currentStageData = stage;
        rootPanel.SetActive(true);

        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(() =>
        {
            int cost = stage.requiredEnergy > 0 ? stage.requiredEnergy : 10;
            if (!energyManager.Instance.TryConsumeMeat(cost))
            {
                Debug.Log("에너지가 부족합니다!");
                // 팝업 등 추가 처리
                return;
            }
            StageEnter(stage);
        });
        // UI에 cost(고기 소모량) 표시
        // enterButton이나 따로 TMP_Text로 cost 표시
        costText.text = $"{stage.requiredEnergy}"; // 예시: 10
        
        // 1. 모든 그룹 비활성
        foreach (var group in scoreGroups)
            group.SetActive(false);

        // 2. stage.stageId에 따라 그룹 활성화
        if (stage.stageId == "stage_1_1")
            scoreGroups[0].SetActive(true);
        else if (stage.stageId == "stage_1_2")
            scoreGroups[1].SetActive(true);
        // 필요에 따라 else if 더 추가

        // 나머지 기존 코드 (타이틀, 적, 보상 등)
        titleText.text = $"{stage.stageName}";
        RefreshEnemyList(stage.enemyIds);
        RefreshRewardList(stage.rewards, stage.stageId);

        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(() =>
        {
            StageEnter(stage);
        });
    }

    // ⭐⭐ 이 함수가 핵심!
    private void SetScoreGroupActive(string stageId)
    {
        foreach (var group in scoreGroups)
        {
            // 예시: group의 이름에 stageId가 포함되어 있으면 활성화, 아니면 비활성화
            bool active = group.name.Contains(stageId);
            group.SetActive(active);
        }
    }

    private void RefreshEnemyList(List<int> enemyIds)
    {
        foreach (Transform child in enemyListParent) Destroy(child.gameObject);
        foreach (var id in enemyIds)
        {
            var go = Instantiate(enemyIconPrefab, enemyListParent);
            // ⬇️ 데이터로부터 이미지 찾아서 할당
            var icon = enemyIconDB.GetIconById(id);
            if (icon != null)
                go.GetComponentInChildren<Image>().sprite = icon;

            go.GetComponentInChildren<TMPro.TMP_Text>().text = DataManager.Instance.UnitStatTable[id].Name;
        }
    }
    private void RefreshRewardList(List<RewardData> rewards, string stageId)
    {
        foreach (Transform child in rewardListParent) Destroy(child.gameObject);

        string gemKey = $"gemReward_{stageId}";
        bool gemReceived = PlayerPrefs.GetInt(gemKey, 0) == 1;

        foreach (var reward in rewards)
        {
            // 이미 gem 보상을 받았다면 리스트에서 빼고 표시하지 않음
            if (reward.type == "gem" && gemReceived)
                continue;

            GameObject go = null;
            switch (reward.type)
            {
                case "gold":
                    go = Instantiate(moneyIcon, rewardListParent);
                    go.GetComponentInChildren<TMP_Text>().text = reward.amount.ToString();
                    break;
                case "gem":
                    go = Instantiate(gemIcon, rewardListParent);
                    go.GetComponentInChildren<TMP_Text>().text = reward.amount.ToString();
                    break;
                case "item":
                    go = Instantiate(rewardIconPrefab, rewardListParent);
                    go.GetComponentInChildren<TMP_Text>().text = $"{reward.itemId}";
                    break;
            }
        }
    }
    private void StageEnter(StageData stage)
    {
        GameSession.Instance.currentStageId = stage.stageId;
        GameSession.Instance.currentStageData = stage; // ⭐ 반드시 할당
        // SceneManager.LoadScene("InGame");
    }
    
    public void OnStageClear(string stageId, int clearStars)
    {
        StageStarSaveUtil.SaveStarCount(stageId, clearStars);

        var allButtons = FindObjectsOfType<StageButton>();
        foreach (var btn in allButtons)
            if (btn.stageId == stageId)
                btn.RefreshStarUI();
    }
}
