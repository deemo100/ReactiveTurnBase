using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public string stageId;
    public Image[] starImages; // 0~2, 3개 별

    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnStageButtonClick);
        RefreshStarUI();
    }

    void OnEnable()
    {
        RefreshStarUI();
    }

    public void RefreshStarUI()
    {
        int starCount = StageStarSaveUtil.LoadStarCount(stageId);
        for (int i = 0; i < starImages.Length; i++)
            starImages[i].color = (i < starCount) ? Color.white : Color.black;
    }

    public void OnStageButtonClick()
    {
        var stageData = StageDataManager.Instance.StagesFindById(stageId);
        if (stageData == null)
        {
            Debug.LogWarning($"스테이지 {stageId} 데이터 없음");
            return;
        }
        FindObjectOfType<StageInfoPanelManager>().Show(stageData, this);
    }
}