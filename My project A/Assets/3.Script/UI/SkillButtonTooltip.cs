using Game.Input;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("수동 입력 설명(없어도 됨)")]
    [TextArea]
    public string tooltipText;  // Inspector 수동 입력 (없으면 skillData로 자동 생성)

    [Header("SkillData 연결")]
    public SkillData skillData; // Inspector에서 할당하거나 코드에서 동적 할당

    private string cachedTooltip; // 실제로 쓸 설명(최종)

    void Start()
    {
        UpdateTooltipText();
    }

    // 코드에서 SkillData를 변경하고 싶을 때 호출
    public void SetSkill(SkillData data)
    {
        skillData = data;
        UpdateTooltipText();
    }

    private void UpdateTooltipText()
    {
        if (skillData != null)
        {
            cachedTooltip = 
                $"{skillData.Name}\n" +
                $"코스트: {skillData.Cost}\n" +
                $"효과: {skillData.Description}";
        }
        else if (!string.IsNullOrEmpty(tooltipText))
        {
            cachedTooltip = tooltipText;
        }
        else
        {
            cachedTooltip = "(설명 없음)";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 원하는 위치로 고정(예: 좌측 하단)
        Vector2 fixedPos = new Vector2(30, -30); // 화면 고정 위치
        UIManager.Instance.ShowTooltip(cachedTooltip, fixedPos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideTooltip();
    }
}