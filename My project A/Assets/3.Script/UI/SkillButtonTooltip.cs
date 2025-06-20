using Game.Input;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("수동 입력 설명(없어도 됨)")]
    [TextArea]
    public string tooltipText;

    [Header("SkillData 연결")]
    public SkillData skillData;

    private string cachedTooltip;

    // Start/OnEnable 모두 필요 없음

    public void SetManualTooltip(string text)
    {
        skillData = null;
        cachedTooltip = text;
    }

    public void SetSkill(SkillData data)
    {
        skillData = data;
        if (skillData != null)
        {
            cachedTooltip =
                $"<b>{skillData.Name}</b>\n" +
                $"<size=90%>코스트: {skillData.Cost}\n" +
                $"타겟: {skillData.TargetType}\n" +
                $"효과: {skillData.Description}\n" +
                $"공격력/회복량: {skillData.Power}</size>";
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

    public string GetTooltip()
    {
        return cachedTooltip;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[Tooltip] OnPointerEnter: cachedTooltip={cachedTooltip}");
        Vector2 fixedPos = new Vector2(30, -30);
        UIManager.Instance.ShowTooltip(cachedTooltip, fixedPos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideTooltip();
    }
}