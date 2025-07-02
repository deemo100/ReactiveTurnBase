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
    public GameObject rewardIconPrefab;

    // ⭐⭐ [추가] 여러 스테이지별 별 그룹 (score1, score2 등)
    [Header("별 UI 그룹")]
    public GameObject[] scoreGroups; 

    // 스테이지 데이터 참조
    private StageData currentStageData;

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
        RefreshRewardList(stage.rewardIds);

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
            go.GetComponentInChildren<TMP_Text>().text = DataManager.Instance.UnitStatTable[id].Name;
        }
    }
    private void RefreshRewardList(List<int> rewardIds)
    {
        foreach (Transform child in rewardListParent) Destroy(child.gameObject);
        foreach (var id in rewardIds)
        {
            var go = Instantiate(rewardIconPrefab, rewardListParent);
            go.GetComponentInChildren<TMP_Text>().text = $"보상{id}";
        }
    }

    private void StageEnter(StageData stage)
    {
        GameSession.Instance.currentStageId = stage.stageId;
        Debug.Log($"{stage.stageName} 입장!");
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
