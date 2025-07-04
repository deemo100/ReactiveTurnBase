using UnityEngine;
using System;

public class energyManager : MonoBehaviour
{
    public static energyManager Instance { get; private set; }

    public const int Maxenergy = 100;
    public int Currentenergy { get; private set; }

    public float recoveryInterval = 60f; // 10초마다 1 회복
    private float recoveryTimer = 0f;

    // Meat 변경시 UI에 알릴 이벤트
    public event Action<int> OnMeatChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadMeat();
    }

    void Update()
    {
        // 자동 회복
        if (Currentenergy < Maxenergy)
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= recoveryInterval)
            {
                recoveryTimer = 0;
                AddMeat(1);
            }
        }
    }

    public bool TryConsumeMeat(int amount)
    {
        if (Currentenergy < amount)
            return false;

        Currentenergy -= amount;
        SaveMeat();
        OnMeatChanged?.Invoke(Currentenergy);
        return true;
    }

    public void AddMeat(int amount)
    {
        int prev = Currentenergy;
        Currentenergy = Mathf.Clamp(Currentenergy + amount, 0, Maxenergy);

        if (Currentenergy != prev)
        {
            SaveMeat();
            OnMeatChanged?.Invoke(Currentenergy);
        }
    }

    void LoadMeat()
    {
        Currentenergy = PlayerPrefs.GetInt("meat", Maxenergy);
        OnMeatChanged?.Invoke(Currentenergy);
    }

    void SaveMeat()
    {
        PlayerPrefs.SetInt("meat", Currentenergy);
        PlayerPrefs.Save();
    }
}