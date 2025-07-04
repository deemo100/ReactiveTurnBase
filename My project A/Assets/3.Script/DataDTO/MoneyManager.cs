using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    public int Gold { get; private set; }
    public int Gem { get; private set; }
    
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
        Gold += value;
        Save();
        OnGoldChanged?.Invoke(Gold);
    }
    public void AddGem(int value)
    {
        Gem += value;
        Save();
        OnGemChanged?.Invoke(Gem);
    }
    public bool TrySpendGold(int value)
    {
        if (Gold < value) return false;
        Gold -= value;
        Save();
        OnGoldChanged?.Invoke(Gold);
        return true;
    }
    public bool TrySpendGem(int value)
    {
        if (Gem < value) return false;
        Gem -= value;
        Save();
        OnGemChanged?.Invoke(Gem);
        return true;
    }
    private void Save()
    {
        PlayerPrefs.SetInt("gold", Gold);
        PlayerPrefs.SetInt("gem", Gem);
        PlayerPrefs.Save();
    }
    private void Load()
    {
        Gold = PlayerPrefs.GetInt("gold", 0);
        Gem  = PlayerPrefs.GetInt("gem", 0);
        OnGoldChanged?.Invoke(Gold);
        OnGemChanged?.Invoke(Gem);
    }
}
