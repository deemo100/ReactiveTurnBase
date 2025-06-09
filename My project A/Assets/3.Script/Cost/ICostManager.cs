
public interface ICostManager
{
    int  CurrentCost { get; }
    int  MaxCost     { get; }
    bool CanSpend(int amount);
    bool Spend(int amount);
    void Recover(int amount);
}