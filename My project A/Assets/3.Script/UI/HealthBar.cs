using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private Unit targetUnit;

    public void Initialize(Unit unit, Image image)
    {
        targetUnit = unit;
        fillImage = image;
        UpdateBar();
    }

    public void UpdateBar()
    {
        fillImage.fillAmount = Mathf.Clamp01((float)targetUnit.HP / targetUnit.MaxHP);
    }

    // 🔽 새로 추가
    public void SetHealth(float normalized)
    {
        Debug.Log($"[HealthBarFollower] SetHealth: {normalized}");
        fillImage.fillAmount = Mathf.Clamp01(normalized);
    }
}