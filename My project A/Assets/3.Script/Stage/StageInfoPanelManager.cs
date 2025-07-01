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

    // 스테이지 데이터 참조
    private StageData currentStageData;

    void Awake()
    {
        rootPanel.SetActive(false); // 기본 비활성화
        closeButton.onClick.AddListener(() => rootPanel.SetActive(false));
    }

    // 외부에서 호출 (예: 스테이지 버튼 클릭 시)
    public void Show(StageData stage)
    {
        currentStageData = stage;
        rootPanel.SetActive(true);

        titleText.text = $"{stage.stageName}";
        RefreshEnemyList(stage.enemyIds);
        RefreshRewardList(stage.rewardIds);

        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(() =>
        {
            // → 실제 인게임 진입 로직 호출
            StageEnter(stage);
        });
    }

    private void RefreshEnemyList(List<int> enemyIds)
    {
        foreach (Transform child in enemyListParent) Destroy(child.gameObject);
        foreach (var id in enemyIds)
        {
            var go = Instantiate(enemyIconPrefab, enemyListParent);
            // 아이콘/이름 등 세팅
            go.GetComponentInChildren<TMP_Text>().text = DataManager.Instance.UnitStatTable[id].Name;
            // 아이콘 이미지 등도 필요시 추가
        }
    }
    private void RefreshRewardList(List<int> rewardIds)
    {
        foreach (Transform child in rewardListParent) Destroy(child.gameObject);
        foreach (var id in rewardIds)
        {
            var go = Instantiate(rewardIconPrefab, rewardListParent);
            // 보상 아이콘/텍스트 등 세팅
            go.GetComponentInChildren<TMP_Text>().text = $"보상{id}";
        }
    }

    private void StageEnter(StageData stage)
    {
        // 인게임 씬 이동, 선택 파티 저장 등
        Debug.Log($"{stage.stageName} 입장!");
        // ... 예시: SceneManager.LoadScene("InGame");
    }
}
