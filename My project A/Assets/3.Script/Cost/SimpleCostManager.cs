using UnityEngine;

public class SimpleCostManager : MonoBehaviour, ICostManager
{
    [SerializeField] private int _maxCost = 8;
    private int _currentCost;

    public int CurrentCost => _currentCost;
    public int MaxCost     => _maxCost;

    private void Awake()
    {
        _currentCost = _maxCost;
    }

    public bool CanSpend(int amount) => amount <= _currentCost;

    public bool Spend(int amount)
    {
        if (!CanSpend(amount)) return false;
        _currentCost -= amount;
        return true;
    }

    public void Recover(int amount)
    {
        _currentCost = Mathf.Min(_currentCost + amount, _maxCost);
    }
}