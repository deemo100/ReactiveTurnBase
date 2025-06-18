using UnityEngine;

public class PlayerUnit : Unit
{
    public bool HasActedThisTurn { get; private set; }
    public bool IsSelected { get; private set; }

    [Header("유닛 대표 무기 아이콘 (Inspector에서 할당)")]
    public Sprite WeaponIcon;  // Inspector에서 할당(드래그)
    public Sprite SkillIcon; 
    
    public void MarkActed()
    {
        HasActedThisTurn = true;
        SetSelected(false);
        Debug.Log($"[PlayerUnit] {UnitName} MarkActed 호출, HasActedThisTurn: {HasActedThisTurn}");
    }

    public void ResetTurn()
    {
        HasActedThisTurn = false;
        SetSelected(false);
        Debug.Log($"[PlayerUnit] {UnitName} ResetTurn 호출, HasActedThisTurn: {HasActedThisTurn}");
    }
    

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        Debug.Log($"[PlayerUnit] SetSelected 호출: {UnitName} 지정 상태 → {IsSelected}");
    }
    public override void Init(UnitStat stat)
    {
        base.Init(stat);
        if (DataManager.Instance.SkillTable.TryGetValue(stat.SkillId, out var skill))
        {
            SkillData = skill;
            Debug.Log($"[PlayerUnit] {UnitName} SkillData 할당 완료! → {skill.Name}");
        }
        else
        {
            Debug.LogWarning($"[PlayerUnit] {UnitName} SkillData 할당 실패! (SkillId={stat.SkillId})");
        }
    }
    
}