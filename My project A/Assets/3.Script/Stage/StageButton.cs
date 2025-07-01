using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public string stageId; // 인스펙터에서 직접 입력
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnStageButtonClick);
    }

    public void OnStageButtonClick()
    {
        // 1. StageDataManager에서 해당 id의 StageData 가져오기
        var stageData = StageDataManager.Instance.StagesFindById(stageId);
        if (stageData == null)
        {
            Debug.LogWarning($"스테이지 {stageId} 데이터 없음");
            return;
        }

        // 2. StageInfoPanelManager로 데이터 전달 & 패널 오픈
        FindObjectOfType<StageInfoPanelManager>().Show(stageData);
    }
}