using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unit(플레이어/적 상관없이) HP/MaxHP를 위에 배치된 UI Image(filled)로 표시.
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("바꿔야 할 Target Unit")]
    [Tooltip("Inspector에서 드래그하세요")]
    [SerializeField] private Unit _targetUnit;

    [Header("Fill용 Image (Filled 타입)")]
    [Tooltip("Inspector에서 자식 fill_bar 오브젝트의 Image를 드래그하세요")]
    [SerializeField] private Image _fillImage;

    void Update()
    {
        if (_targetUnit == null || _fillImage == null) return;

        // 0~1 사이 값으로 만들어서 fillAmount에 대입
        float ratio = (float)_targetUnit.HP / _targetUnit.MaxHP;
        _fillImage.fillAmount = Mathf.Clamp01(ratio);
    }
}