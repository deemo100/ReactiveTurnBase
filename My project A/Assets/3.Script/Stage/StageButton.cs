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
        if (_button == null)
        {
            Debug.LogError($"[StageButton] Button 컴포넌트가 없습니다! 오브젝트: {gameObject.name}", this);
            return;
        }
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
        {
            bool active = i < starCount;
            starImages[i].color = active ? Color.white : Color.black;
        }
    }

    public void OnStageButtonClick()
    {
        var stageData = StageDataManager.Instance.StagesFindById(stageId);
        if (stageData == null)
        {
            Debug.LogWarning($"스테이지 {stageId} 데이터 없음");
            return;
        }
        // ★★★ 이 부분 추가! 선택한 stageId를 GameSession에 저장 ★★★
        GameSession.Instance.currentStageId = stageId;

        FindObjectOfType<StageInfoPanelManager>().Show(stageData, this);
    }
}