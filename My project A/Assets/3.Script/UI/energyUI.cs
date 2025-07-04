using TMPro;
using UnityEngine;

public class energyUI : MonoBehaviour
{
    public TMP_Text energyText;

    void Start()
    {
        energyManager.Instance.OnMeatChanged += UpdateMeatText;
        UpdateMeatText(energyManager.Instance.Currentenergy);
    }

    void OnDestroy()
    {
        if (energyManager.Instance != null)
            energyManager.Instance.OnMeatChanged -= UpdateMeatText;
    }

    void UpdateMeatText(int value)
    {
        energyText.text = $"{value} / {energyManager.Maxenergy}";
    }
}