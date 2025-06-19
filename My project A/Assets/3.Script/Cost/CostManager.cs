using System;
using UnityEngine;

public class CostManager : MonoBehaviour
{
    public event Action<int, int> OnCostChanged;

    private int _currentCost;
    private int _maxCost = 8;
    public int CurrentCost => _currentCost;
    public int MaxCost => _maxCost;
    
    /// <summary>
    /// 초기 코스트 설정. Init만 불러주면 됩니다.
    /// </summary>
    public void Init(int startCost)
    {
        _currentCost = Mathf.Clamp(startCost, 0, _maxCost);
        OnCostChanged?.Invoke(_currentCost, _maxCost);
    }

    public bool CanUse(int amount) => _currentCost >= amount; // ✅ 추가!
    
    /// <summary>
    /// 코스트 사용 시 호출. 사용 가능하면 true, 아니면 false를 반환.
    /// </summary>
    public bool Use(int amount)
    {
        if (_currentCost < amount) return false;
        _currentCost -= amount;
        OnCostChanged?.Invoke(_currentCost, _maxCost);
        return true;
    }

    /// <summary>
    /// 턴이 끝날 때마다 호출해서 코스트를 채워줍니다.
    /// </summary>
    public void Refill(int amount)
    {
        _currentCost = Mathf.Min(_currentCost + amount, _maxCost);
        OnCostChanged?.Invoke(_currentCost, _maxCost);
    }
}