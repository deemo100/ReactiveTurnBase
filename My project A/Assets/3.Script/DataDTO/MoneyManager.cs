using System;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    private int gold;
    private int gem;

    public int Gold => gold;
    public int Gem => gem; // 직접 게터만 사용

    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void AddGold(int value)
    {
        gold += value;
        Save();
        OnGoldChanged?.Invoke(gold);
    }

    public void AddGem(int value)
    {
        gem += value;
        Save();
        OnGemChanged?.Invoke(gem);
    }

    public bool TryConsumeGem(int amount)
    {
        if (gem < amount)
        {
            Debug.LogWarning($"[MoneyManager] 보석 부족! 현재: {gem}, 필요: {amount}");
            return false;
        }
        gem -= amount;
        Save();
        OnGemChanged?.Invoke(gem);
        Debug.Log($"[MoneyManager] 보석 차감: {amount}, 남은 보석: {gem}");
        return true;
    }

    private void Save()
    {
        PlayerPrefs.SetInt("gold", gold);
        PlayerPrefs.SetInt("gem", gem);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        gold = PlayerPrefs.GetInt("gold", 0);
        gem  = PlayerPrefs.GetInt("gem", 0);
        OnGoldChanged?.Invoke(gold);
        OnGemChanged?.Invoke(gem);
        Debug.Log($"[MoneyManager] Load 완료 - 골드: {gold}, 보석: {gem}");
    }
}