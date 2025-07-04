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

    // 승리 패널 오픈 시 반드시 Setup 호출!
    public void Setup(string clearStageId, int clearStars)
    {
        stageId = clearStageId;
        starCount = clearStars;
        victoryStarController.SetStarsByCount(starCount);

        // ⭐⭐⭐ 여기서 한 번만 보상 지급!
        Debug.Log("보상 지급 시도: " + GameSession.Instance.currentStageData.stageId);
        RewardManager.Instance.GiveStageReward(GameSession.Instance.currentStageData, starCount);

        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(OnRetryClicked);

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExitClicked);
    }

    public void OnRetryClicked()
    {
        // ⭐ 여기에 starCount 사용
        StageStarSaveUtil.SaveStarCount(GameSession.Instance.currentStageId, starCount);
        SceneManager.LoadScene("inGame");
    }

    public void OnExitClicked()
    {
        StageStarSaveUtil.SaveStarCount(GameSession.Instance.currentStageId, GameSession.Instance.lastClearStarCount);
        int check = StageStarSaveUtil.LoadStarCount(GameSession.Instance.currentStageId);
        Debug.Log($"OnExitClicked 후 즉시 체크: {GameSession.Instance.currentStageId}_stars = {check}");

        SceneManager.LoadScene("mainmenu");
    }

    private void SaveAndChangeScene(string sceneName)
    {
        // ⭐ 별 갯수 저장
        StageStarSaveUtil.SaveStarCount(stageId, starCount);
        PlayerPrefs.Save();

        // (추가: 필요하면 게임 세션 등도 정리)
        // 씬 이동
        SceneManager.LoadScene(sceneName);
    }
}