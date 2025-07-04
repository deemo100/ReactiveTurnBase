using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryPanelManager : MonoBehaviour
{
    public VictoryStarController victoryStarController;
    public Button retryButton;
    public Button exitButton;

    private string stageId;
    private int starCount;

    public Image[] rewardSlots;
    public Sprite goldIcon, gemIcon, itemIcon;

    public GameObject energyPopup;
    public Button popupCancelButton;
    public Button popupConfirmButton;

    // ⭐ 중복 팝업 방지용 플래그
    private bool isEnergyPopupActive = false;

    public void Setup(string clearStageId, int clearStars)
    {
        stageId = clearStageId;
        starCount = clearStars;

        var stageData = GameSession.Instance.currentStageData;
        int cost = stageData.requiredEnergy > 0 ? stageData.requiredEnergy : 10;
        Debug.Log($"[VictoryPanel] 클리어 성공! 에너지 차감 시도: {cost}, 현재:{energyManager.Instance.Currentenergy}");

        // 이미 깎인 적이 없으면 차감
        if (!energyManager.Instance.TryConsumeenergy(cost))
            Debug.LogWarning("에너지가 부족합니다(비정상 상황)!");

        victoryStarController.SetStarsByCount(starCount);
        RewardManager.Instance.GiveStageReward(stageData, starCount);
        ShowClearRewards(stageData, starCount);

        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(OnRetryClicked);

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExitClicked);

        // 팝업 초기 상태
        energyPopup.SetActive(false);
        isEnergyPopupActive = false;
    }

    public void OnRetryClicked()
    {
        int cost = GameSession.Instance.currentStageData.requiredEnergy > 0
            ? GameSession.Instance.currentStageData.requiredEnergy : 10;

        if (energyManager.Instance.Currentenergy >= cost)
        {
            if (energyManager.Instance.TryConsumeenergy(cost))
                GoToStageAgain();
            else
                Debug.LogWarning("에너지가 충분한데 차감 실패?");
            return;
        }

        int gemCost = 10;
        if (MoneyManager.Instance.Gem >= gemCost)
        {
            if (isEnergyPopupActive) return;
            ShowEnergyPopup(() =>
            {
                if (MoneyManager.Instance.TryConsumeGem(gemCost))
                {
                    // 오버 충전 (남은 에너지 + 최대치)
                    energyManager.Instance.Currentenergy += energyManager.MaxEnergy;

                    // 스테이지 코스트 차감, 입장
                    if (energyManager.Instance.TryConsumeenergy(cost))
                        GoToStageAgain();
                    else
                        Debug.LogWarning("충전 후에도 에너지 부족!");
                }
                else
                {
                    Debug.Log("보석이 부족합니다!");
                }
            });
        }
        else
        {
            Debug.Log("에너지도, 보석도 부족합니다!");
        }
    }

    public void OnExitClicked()
    {
        StageStarSaveUtil.SaveStarCount(GameSession.Instance.currentStageId, GameSession.Instance.lastClearStarCount);
        int check = StageStarSaveUtil.LoadStarCount(GameSession.Instance.currentStageId);
        Debug.Log($"OnExitClicked 후 즉시 체크: {GameSession.Instance.currentStageId}_stars = {check}");

        SceneManager.LoadScene("mainmenu");
    }

    public void ShowClearRewards(StageData stage, int starCount)
    {
        // 1. 슬롯 초기화 (모두 비활성)
        for (int i = 0; i < rewardSlots.Length; i++)
            rewardSlots[i].gameObject.SetActive(false);

        int slotIdx = 0;
        foreach (var reward in stage.rewards)
        {
            if (reward.type == "gem")
            {
                string gemKey = $"gemReward_{stage.stageId}";
                bool gemReceived = PlayerPrefs.GetInt(gemKey, 0) == 1;
                if (gemReceived)
                    continue;
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

        for (int i = slotIdx; i < rewardSlots.Length; i++)
            rewardSlots[i].gameObject.SetActive(false);
    }

    // ** 팝업은 중복 호출 방지, 확인/취소 명확히 분리 **
    void ShowEnergyPopup(System.Action onConfirm)
    {
        if (isEnergyPopupActive) return;
        isEnergyPopupActive = true;
        energyPopup.SetActive(true);

        popupCancelButton.onClick.RemoveAllListeners();
        popupConfirmButton.onClick.RemoveAllListeners();

        popupCancelButton.onClick.AddListener(() =>
        {
            energyPopup.SetActive(false);
            isEnergyPopupActive = false;
        });
        popupConfirmButton.onClick.AddListener(() =>
        {
            Debug.Log("예 버튼 클릭됨");
            energyPopup.SetActive(false);
            isEnergyPopupActive = false;
            onConfirm?.Invoke();
        });
    }


    private void GoToStageAgain()
    {
        StageStarSaveUtil.SaveStarCount(GameSession.Instance.currentStageId, starCount);
        SceneManager.LoadScene("inGame");
    }
}
