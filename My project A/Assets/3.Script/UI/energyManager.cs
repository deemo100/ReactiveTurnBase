using UnityEngine;
using System;

public class energyManager : MonoBehaviour
{
    public static energyManager Instance { get; private set; }


   
    public const int MaxEnergy = 100; // ← 대문자 E로 변경!
    public int Currentenergy { get; set; }

    public float recoveryInterval = 60f; // 10초마다 1 회복
    private float recoveryTimer = 0f;

    // Meat 변경시 UI에 알릴 이벤트
    public event Action<int> OnenergyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Loadenergy();
    }

    void Update()
    {
        // 자동 회복
        if (Currentenergy < MaxEnergy)
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= recoveryInterval)
            {
                recoveryTimer = 0;
                Addenergy(1);
            }
        }
    }

    public bool TryConsumeenergy(int amount)
    {
        if (Currentenergy < amount)
            return false;

        Currentenergy -= amount;
        Saveenergy();
        OnenergyChanged?.Invoke(Currentenergy);
        return true;
    }

    public void Addenergy(int amount, bool allowOvercharge = false)
    {
        int prev = Currentenergy;
        if (allowOvercharge)
            Currentenergy += amount;
        else
            Currentenergy = Mathf.Clamp(Currentenergy + amount, 0, MaxEnergy);

        if (Currentenergy != prev)
        {
            Saveenergy();
            OnenergyChanged?.Invoke(Currentenergy);
        }
    }

    void Loadenergy()
    {
        Currentenergy = PlayerPrefs.GetInt("meat", MaxEnergy);
        OnenergyChanged?.Invoke(Currentenergy);
    }

    void Saveenergy()
    {
        PlayerPrefs.SetInt("meat", Currentenergy);
        PlayerPrefs.Save();
    }
    
    public void FillToMax()
    {
        Currentenergy = MaxEnergy;
        Saveenergy();
        OnenergyChanged?.Invoke(Currentenergy);
    }
    
    public void FillOverCharge()
    {
        Currentenergy = MaxEnergy + Currentenergy;
        Saveenergy();
        OnenergyChanged?.Invoke(Currentenergy);
    }
    
}