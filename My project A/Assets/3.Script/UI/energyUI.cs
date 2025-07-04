using TMPro;
using UnityEngine;

public class energyUI : MonoBehaviour
{
    public TMP_Text energyText;

    void Start()
    {
        energyManager.Instance.OnenergyChanged += UpdateMeatText;
        UpdateMeatText(energyManager.Instance.Currentenergy);
    }

    void OnDestroy()
    {
        if (energyManager.Instance != null)
            energyManager.Instance.OnenergyChanged -= UpdateMeatText;
    }

    void UpdateMeatText(int value)
    {
        energyText.text = $"{value} / {energyManager.MaxEnergy}";
    }
}