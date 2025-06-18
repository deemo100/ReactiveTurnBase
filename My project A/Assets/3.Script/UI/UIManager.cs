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
    
    [Header("버튼 아이콘 Image")]
    public Image attackIconImage;  // 공격 아이콘
    public Image skillIconImage;   // 스킬 아이콘
    
    [Header("공격/스킬 하이라이트(Effect 등)")]
    public GameObject inputAttack; // 공격시 활성화
    public GameObject inputSkill;  // 스킬시 활성화 (원하면 추가)
    
    void Awake()
    {
        Instance = this;
        
        // **처음에 버튼 패널 비활성화**
        if (actionButtonPanel != null)
            actionButtonPanel.SetActive(false);

        attackButton.onClick.AddListener(() => 
            InputServiceNew.Instance.EnterAttackMode());
        
        skill1Button.onClick.AddListener(() => 
            InputServiceNew.Instance.EnterSkillMode(DataManager.Instance.GetSkillById(1)));
        
        if (inputAttack) inputAttack.SetActive(false);
        if (inputSkill) inputSkill.SetActive(false);
    }
    
    public void SetAttackIcon(Sprite icon)
    {
        if (attackIconImage) attackIconImage.sprite = icon;
    }
    public void SetSkillIcon(Sprite icon)
    {
        if (skillIconImage) skillIconImage.sprite = icon;
    }
    
    public void SetAttackHighlight(bool active)
    {
        if (inputAttack) inputAttack.SetActive(active);
    }
    public void SetSkillHighlight(bool active)
    {
        if (inputSkill) inputSkill.SetActive(active);
    }
    
    public void ShowActionButtons(PlayerUnit unit)
    {
        actionButtonPanel.SetActive(true);
        attackButton.interactable = !unit.HasActedThisTurn;
        skill1Button.interactable = !unit.HasActedThisTurn;
        SetAttackIcon(unit.WeaponIcon); // 일반 공격 아이콘
        SetSkillIcon(unit.SkillIcon);   // 스킬 아이콘
    }
    public void HideActionButtons()
    {
        actionButtonPanel.SetActive(false);
        SetAttackHighlight(false);
        SetSkillHighlight(false);
    }

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