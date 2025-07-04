using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public TMP_Text goldText;
    public TMP_Text gemText;

    private void Start()
    {
        MoneyManager.Instance.OnGoldChanged += UpdateGold;
        MoneyManager.Instance.OnGemChanged += UpdateGem;
        UpdateGold(MoneyManager.Instance.Gold);
        UpdateGem(MoneyManager.Instance.Gem);
    }

    private void OnDestroy()
    {
        if (MoneyManager.Instance == null) return;
        MoneyManager.Instance.OnGoldChanged -= UpdateGold;
        MoneyManager.Instance.OnGemChanged  -= UpdateGem;
    }

    private void UpdateGold(int value)
    {
        goldText.text = $"{value}";
    }
    private void UpdateGem(int value)
    {
        gemText.text = $"{value}";
    }
}