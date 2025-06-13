using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollower : MonoBehaviour
{
    [Header("할당할 UI 컴포넌트")]
    [SerializeField] private Image    fillImage;

    private Transform    _target;
    private Vector3      _worldOffset;
    private RectTransform _rectTransform;
    private Camera       _mainCam;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _mainCam       = Camera.main;
    }

    /// <summary>
    /// 스폰 직후 한 번만 호출합니다.
    /// </summary>
    public void Initialize(Transform target, Vector3 offset)
    {
        _target      = target;
        _worldOffset = offset;
    }

    void LateUpdate()
    {
        if (_target == null) return;
        // 1) 월드 좌표 → 스크린 좌표
        Vector3 screenPos = _mainCam.WorldToScreenPoint(_target.position + _worldOffset);

        // 2) 캔버스 상의 위치로 반영
        _rectTransform.position = screenPos;
    }

    /// <summary>
    /// 외부에서 체력 %를 세팅해 주면 fill amount에 반영됩니다.
    /// 예: healthBar.SetHealth(currentHP / (float)maxHP);
    /// </summary>
    public void SetHealth(float normalized)
    {
        fillImage.fillAmount = Mathf.Clamp01(normalized);
    }
}