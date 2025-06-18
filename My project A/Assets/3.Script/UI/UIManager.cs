using TMPro; // TextMeshPro 사용 시
using UnityEngine;
using UnityEngine.UI; // Button, Text 타입


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    public TMP_Text turnNumberText;   // Inspector에서 드래그
    public GameObject victoryPanel;   // Inspector에서 승리 패널 드래그
    public GameObject defeatPanel;    // Inspector에서 패배 패널 드래그
    
    public Button attackButton, skill1Button;
    
    // 이거 나중에 잘못된 타겟이나 이미 행동한 유닛입니다라는 텍스트 띄우는 용도 사용
    public TMP_Text targetGuideText;
    
    public GameObject actionButtonPanel;
   
    [Header("액션 버튼 하이라이트/이펙트")]
    [SerializeField] private GameObject attackHighlight; // inputattack 오브젝트 Drag&Drop
    
    void Awake()
    {
        Instance = this;
        attackButton.onClick.AddListener(() => 
            InputServiceNew.Instance.EnterAttackMode());
        
        skill1Button.onClick.AddListener(() => 
            InputServiceNew.Instance.EnterSkillMode(DataManager.Instance.GetSkillById(1)));
    }
    public void SetAttackHighlight(bool active)
    {
        if (attackHighlight != null)
            attackHighlight.SetActive(active);
    }
    
    public void ShowActionButtons(PlayerUnit unit)
    {
        actionButtonPanel.SetActive(true);
        attackButton.interactable = !unit.HasActedThisTurn;
        skill1Button.interactable = !unit.HasActedThisTurn;
    }
    public void HideActionButtons() => actionButtonPanel.SetActive(false);

    public void ShowTargetGuide(string msg)
    {
        targetGuideText.gameObject.SetActive(true);
        targetGuideText.text = msg;
    }
    public void HideTargetGuide()
    {
        targetGuideText.gameObject.SetActive(false);
    }
    
    // --- 턴 표시 ---
    public void UpdateTurnText(int turnNum)
    {
        if (turnNumberText != null)
            turnNumberText.text = $"TURN {turnNum}";
    }

    // --- 승리/패배 패널 표시 ---
    public void ShowVictory()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }
    public void ShowDefeat()
    {
        if (defeatPanel != null) defeatPanel.SetActive(true);
    }
    
}