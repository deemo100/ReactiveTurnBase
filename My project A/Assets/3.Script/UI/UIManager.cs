using Game.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


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

    [Header("코스트 부족 알림")]
    [SerializeField] private TMP_Text costWarningText; // CostText 오브젝트 연결
   
    private Coroutine warningCoroutine;
    
    void Awake()
    {
        Instance = this;
        if (actionButtonPanel != null)
            actionButtonPanel.SetActive(false);
        if (costWarningText != null)
            costWarningText.gameObject.SetActive(false); // 시작 시 숨김
        
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
        // 1. 툴팁 먼저 세팅
        var attackTooltip = attackButton.GetComponent<SkillButtonTooltip>();
        if (attackTooltip != null)
        {
            string desc = $"<b>일반 공격</b>\n<size=90%>자신의 공격력 100% 피해를 적 1명에게 가합니다.";
            attackTooltip.SetManualTooltip(desc);
        }
        
        var skillTooltip = skill1Button.GetComponent<SkillButtonTooltip>();
        if (skillTooltip != null)
            skillTooltip.SetSkill(unit.SkillData);
        
        actionButtonPanel.SetActive(true);
        attackButton.interactable = !unit.HasActedThisTurn;
        skill1Button.interactable = !unit.HasActedThisTurn;

        SetAttackIcon(unit.WeaponIcon);
        SetSkillIcon(unit.SkillIcon);

        attackButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(() =>
        {
            InputServiceNew.Instance.EnterAttackMode();
            UIManager.Instance.HideTooltip(); // 설명창 숨기기
        });
        
        // 기존 리스너 모두 제거 후 새로 추가
        skill1Button.onClick.RemoveAllListeners();
        skill1Button.onClick.AddListener(() =>
        {
            InputServiceNew.Instance.EnterSkillMode(unit.SkillData);
            UIManager.Instance.HideTooltip(); // ← 버튼 누르면 설명 패널도 즉시 숨김
        });
        
    }

    // ==== 코스트 부족 알림 ==== //
    public void ShowCostWarning(string message = "코스트가 부족합니다!", float duration = 3f, float blinkSpeed = 0.3f)
    {
        Debug.Log($"[ShowCostWarning 호출!] message={message}, costWarningText={costWarningText}");
        if (costWarningText == null)
        {
            Debug.LogError("[ShowCostWarning] costWarningText가 null입니다! Inspector에서 연결하세요.");
            return;
        }
        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        warningCoroutine = StartCoroutine(BlinkCostWarning(message, duration, blinkSpeed));
    }

    private IEnumerator BlinkCostWarning(string text, float duration, float blinkSpeed)
    {
        costWarningText.text = text;
        costWarningText.gameObject.SetActive(true);

        float timer = 0;
        bool visible = true;

        while (timer < duration)
        {
            costWarningText.alpha = visible ? 1f : 0.2f;
            visible = !visible;
            yield return new WaitForSeconds(blinkSpeed);
            timer += blinkSpeed;
        }

        costWarningText.alpha = 1f;
        costWarningText.gameObject.SetActive(false);
        warningCoroutine = null;
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
