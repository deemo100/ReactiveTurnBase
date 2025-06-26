using UnityEngine;
using UnityEngine.UI;

public class GroggyBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private EnemyUnit targetUnit;  // EnemyUnit만 사용

    public void Initialize(EnemyUnit unit, Image image)
    {
        targetUnit = unit;
        fillImage = image;
        UpdateBar();
    }

    public void UpdateBar()
    {
        fillImage.fillAmount = Mathf.Clamp01((float)targetUnit.Groggy / targetUnit.MaxGroggy);
    }

    public void SetGroggy(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);
        fillImage.fillAmount = float.IsNaN(normalized) ? 1 : normalized;
    }
}