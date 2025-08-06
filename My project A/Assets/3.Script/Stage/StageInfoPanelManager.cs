using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class StageInfoPanelManager : MonoBehaviour
{
    [Header("UI 요소")]
    public GameObject rootPanel;
    public TMP_Text titleText;
    public Transform enemyListParent;
    public Transform rewardListParent;
    public Button enterButton;
    public Button closeButton;
    
    [Header("게임 종료 버튼")]
    public GameObject exitPopup;    // Inspector에서 exitpopup 오브젝트 할당
    public Button arrowButton;      // 왼쪽 상단 화살표 버튼
    public Button quitConfirmButton;  // "확인" 버튼
    public Button quitCancelButton;   // "취소" 버튼
    
    [Header("보상 프리팹")]
    public GameObject moneyIcon;
    public GameObject gemIcon;
    public GameObject rewardIconPrefab;

    public EnemyIconDatabase enemyIconDB; 
    
    [Header("오토 클리어 Victory")]
    [SerializeField] private AutoVictoryPanelManager autoVictoryPanelManager;
    
    // [추가] 여러 스테이지별 별 그룹 (score1, score2 등)
    [Header("별 UI 그룹")]
    public GameObject[] scoreGroups; 

    // 스테이지 데이터 참조
    private StageData currentStageData;
    public TMP_Text costText;
    
    public GameObject energyPopup;
    // (적/보상 아이콘용 프리팹)
    public GameObject enemyIconPrefab;
    
    
    void Awake()
    {
        rootPanel.SetActive(false); // 기본 비활성화
        closeButton.onClick.AddListener(() => rootPanel.SetActive(false));
        
        arrowButton.onClick.AddListener(OpenExitPopup);
        quitConfirmButton.onClick.AddListener(OnQuitConfirmed);
        quitCancelButton.onClick.AddListener(CloseExitPopup);
        exitPopup.SetActive(false); // 처음엔 꺼진 상태
    }

    // 외부에서 호출 (예: 스테이지 버튼 클릭 시)
    public void Show(StageData stage, StageButton btn)
    {
        currentStageData = stage;
        rootPanel.SetActive(true);

        // cost UI 표시만 유지 (색상 등 상관 X)
        costText.text = $"{stage.requiredEnergy}";

        // 별 그룹 비활성/활성화 로직 유지
        foreach (var group in scoreGroups)
            group.SetActive(false);

        if (stage.stageId == "stage_1_1")
            scoreGroups[0].SetActive(true);
        else if (stage.stageId == "stage_1_2")
            scoreGroups[1].SetActive(true);
        else if (stage.stageId == "stage_1_3")
            scoreGroups[1].SetActive(true);
        
        // 나머지 UI 세팅
        titleText.text = $"{stage.stageName}";
        RefreshEnemyList(stage.enemyIds);
        RefreshRewardList(stage.rewards, stage.stageId);

        // enterButton 항상 활성 (interactable = true도 추가해도 무방)
        enterButton.interactable = true;
        // 중복 방지! 반드시 RemoveAllListeners
        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(() => TryEnterStage(stage));
    }


    // 이 함수가 핵심!
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
    
    // 별도의 함수로 분리!
    private void TryEnterStage(StageData stage)
    {
        int cost = stage.requiredEnergy > 0 ? stage.requiredEnergy : 10;
        if (energyManager.Instance.Currentenergy < cost)
        {
            ShowEnergyPopup(() => {
                if (MoneyManager.Instance.TryConsumeGem(10))
                {
                    energyManager.Instance.FillOverCharge();
                    // 바로 입장 시도
                    if (energyManager.Instance.Currentenergy >= cost)
                    {
                        energyManager.Instance.TryConsumeenergy(cost);
                        EnterStage(stage);
                    }
                    else
                    {
                        Debug.Log("에너지가 여전히 부족합니다.");
                    }
                }
                else
                {
                    Debug.Log("보석 부족!");
                }
            });
            return;
        }

        // 충분하면 바로 입장
        energyManager.Instance.TryConsumeenergy(cost);
        EnterStage(stage);
    }
    

    private void EnterStage(StageData stage)
    {
        GameSession.Instance.currentStageId = stage.stageId;
        GameSession.Instance.currentStageData = stage;
        Debug.Log($"[StageEnter] {stage.stageId}, meat:{energyManager.Instance.Currentenergy}");
        SceneManager.LoadScene("InGame"); //  꼭 필요!
    }
    
    public void OnStageClear(string stageId, int clearStars)
    {
        StageStarSaveUtil.SaveStarCount(stageId, clearStars);

        var allButtons = FindObjectsOfType<StageButton>();
        foreach (var btn in allButtons)
            if (btn.stageId == stageId)
                btn.RefreshStarUI();
    }
    
    // 에너지 팝업 함수 (VictoryPanelManager와 구조 동일)
    private void ShowEnergyPopup(System.Action onConfirm)
    {
        if (energyPopup == null) return; // Null 체크
        energyPopup.SetActive(true);

        // 버튼 할당 (Hierarchy에서 직접 드래그한 Button 컴포넌트 필요)
        var cancelBtn = energyPopup.transform.Find("CancelButton")?.GetComponent<Button>();
        var confirmBtn = energyPopup.transform.Find("ConfirmButton")?.GetComponent<Button>();

        if (cancelBtn != null)
        {
            cancelBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.AddListener(() =>
            {
                energyPopup.SetActive(false);
            });
        }
        if (confirmBtn != null)
        {
            confirmBtn.onClick.RemoveAllListeners();
            confirmBtn.onClick.AddListener(() =>
            {
                energyPopup.SetActive(false);
                onConfirm?.Invoke();
            });
        }
    }
    
    public void OnClickEnergyOverChargeAndEnter()
    {
        // 현재 스테이지 데이터 필요
        if (currentStageData == null) return;

        int gemCost = 10;
        int cost = currentStageData.requiredEnergy > 0 ? currentStageData.requiredEnergy : 10;

        if (MoneyManager.Instance.TryConsumeGem(gemCost))
        {
            energyManager.Instance.Currentenergy = energyManager.MaxEnergy + energyManager.Instance.Currentenergy;

            // 입장 코스트 차감
            if (energyManager.Instance.TryConsumeenergy(cost))
            {
                EnterStage(currentStageData);
            }
            else
            {
                Debug.Log("에너지 충전 후에도 입장 비용 부족! (이론상 발생 불가)");
            }
        }
        else
        {
            Debug.Log("보석 부족!");
            // 필요시 팝업 닫기 등 추가
            energyPopup.SetActive(false);
        }
    }
    public void OnClickCloseEnergyPopup()
    {
        energyPopup.SetActive(false);
    }
    
    private void OnAutoButtonClicked()
    {
        int star = StageStarSaveUtil.LoadStarCount(currentStageData.stageId);
        if (star < 3)
        {
            // 경고 등 표시
            Debug.Log("3별 클리어 시에만 오토 클리어 가능!");
            return;
        }
        // 보상 지급 (별도 함수 활용)
        GiveClearReward(currentStageData);

        // 오토 Victory 패널 노출
        if (autoVictoryPanelManager != null)
            autoVictoryPanelManager.ShowAutoVictory(currentStageData);
    }
    
    // 오토 클리어 시 보상 지급
    private void GiveClearReward(StageData stage)
    {
        foreach (var reward in stage.rewards)
        {
            switch (reward.type)
            {
                case "gold": MoneyManager.Instance.AddGold(reward.amount); break;
                case "gem": MoneyManager.Instance.AddGem(reward.amount); break;
                // 기타 아이템 등
            }
        }
        // 오토로도 별 획득/갱신(이미 3별이라면 값이 동일하겠지만)
        StageStarSaveUtil.SaveStarCount(stage.stageId, 3);
    }
    
    private void OpenExitPopup()
    {
        exitPopup.SetActive(true);
    }

    private void CloseExitPopup()
    {
        exitPopup.SetActive(false);
    }

    private void OnQuitConfirmed()
    {
        Debug.Log("게임 종료 버튼 클릭됨");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서만 동작
#endif
    }
 
    
    private void OnQuitClicked()
    {
        Debug.Log("게임 종료 버튼 클릭됨");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
}
