using Game.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TMP_Text turnNumberText;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public Button attackButton, skill1Button;
    public TMP_Text targetGuideText;
    public GameObject actionButtonPanel;

    [Header("버튼 아이콘 Image")]
    public Image attackIconImage;
    public Image skillIconImage;

    [Header("공격/스킬 하이라이트(Effect 등)")] 
    public GameObject inputAttack;
    public GameObject inputSkill;

    [Header("툴팁 UI")] [SerializeField] 
    private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;

    void Awake()
    {
        Instance = this;
        if (actionButtonPanel != null)
            actionButtonPanel.SetActive(false);

        attackButton.onClick.AddListener(() =>
            InputServiceNew.Instance.EnterAttackMode());

        skill1Button.onClick.AddListener(() =>
            InputServiceNew.Instance.EnterSkillMode(DataManager.Instance.GetSkillById(1)));

        if (inputAttack) inputAttack.SetActive(false);
        if (inputSkill) inputSkill.SetActive(false);
        HideTooltip();
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

        SetAttackIcon(unit.WeaponIcon);
        SetSkillIcon(unit.SkillIcon);

        // [중요] 버튼에 SkillData 동적 할당!
        var tooltip = skill1Button.GetComponent<SkillButtonTooltip>();
        if (tooltip != null)
            tooltip.SetSkill(unit.SkillData);  // 유닛이 가진 SkillData로 설명 동적 생성
    }

    public void HideActionButtons()
    {
        actionButtonPanel.SetActive(false);
        SetAttackHighlight(false);
        SetSkillHighlight(false);
    }

    // ⭐ SkillData용 툴팁 표시
    public void ShowSkillTooltip(SkillData skill)
    {
        if (skill == null)
        {
            HideTooltip();
            return;
        }

        string desc =
            $"<b>{skill.Name}</b>\n" +
            $"<size=90%>코스트: {skill.Cost}\n" +
            $"타겟: {skill.TargetType}\n" +
            $"효과: {skill.Description}\n" +
            $"공격력/회복량: {skill.Power}</size>";

        tooltipText.text = desc;
        tooltipPanel.SetActive(true);

        // 원하는 위치에 표시 (예시: 고정 좌측 하단)
        tooltipPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, -30);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    public void ShowTooltip(string desc, Vector2 anchoredPos)
    {
        tooltipText.text = desc;
        tooltipPanel.SetActive(true);
        tooltipPanel.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
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

    public void UpdateTurnText(int turnNum)
    {
        if (turnNumberText != null)
            turnNumberText.text = $"TURN {turnNum}";
    }

    public void ShowVictory()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    public void ShowDefeat()
    {
        if (defeatPanel != null) defeatPanel.SetActive(true);
    }
}
