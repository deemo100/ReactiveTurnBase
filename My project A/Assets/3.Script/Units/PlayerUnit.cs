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
     
    }

    public void ResetTurn()
    {
        HasActedThisTurn = false;
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
    }

    public override void Init(UnitStat stat, TeamType team)
    {
        base.Init(stat, team);

        if (DataManager.Instance.SkillTable.TryGetValue(stat.SkillId, out var skill))
        {
            SkillData = skill;
        }
        else
        {
            Debug.LogWarning($"[PlayerUnit] {UnitName} SkillData 할당 실패! (SkillId={stat.SkillId})");
        }
    }
}