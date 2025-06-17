using UnityEngine;

public class PlayerUnit : Unit
{
    public bool HasActedThisTurn { get; private set; }
    public bool IsSelected { get; private set; }

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
}