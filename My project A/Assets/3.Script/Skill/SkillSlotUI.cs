using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class SkillSlotUI : MonoBehaviour, IPointerClickHandler
{
    public event Action<SkillData> OnClicked;

    private Image     _icon;
    private SkillData _data;

    private void Awake()
    {
        _icon = GetComponent<Image>();
    }

    /// <summary>
    /// 슬롯 하나를 스킬 데이터로 초기화
    /// </summary>
    public void Initialize(SkillData data)
    {
        _data = data;
        _icon.sprite = data.icon;
        _icon.color  = Color.white;
    }

    public void OnPointerClick(PointerEventData e)
    {
        OnClicked?.Invoke(_data);
    }

    /// <summary>
    /// 코스트 부족 시 비활성화 처리
    /// </summary>
    public void SetInteractable(bool enabled)
    {
        _icon.color = enabled ? Color.white : Color.gray;
    }
}