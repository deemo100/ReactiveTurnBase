using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollower : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private Transform _target;
    private Vector3 _worldOffset;
    private RectTransform _rectTransform;
    private Camera _mainCam;

    // 🔽 애니메이션용 변수
    private float targetFill = 1f;
    private float animTime = 0.5f;
    private float elapsed = 0f;
    private bool isAnimating = false;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _mainCam = Camera.main;
    }

    public void Initialize(Transform target, Vector3 offset)
    {
        _target = target;
        _worldOffset = offset;
    }

    void LateUpdate()
    {
        if (_target == null) return;
        Vector3 screenPos = _mainCam.WorldToScreenPoint(_target.position + _worldOffset);
        _rectTransform.position = screenPos;

        // 🔽 바 애니메이션 처리
        if (isAnimating && fillImage != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animTime);
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, t);

            if (t >= 1f)
            {
                fillImage.fillAmount = targetFill;
                isAnimating = false;
            }
        }
    }

    // ★ 애니메이션 버전
    public void SetHealth(float normalized)
    {
        normalized = float.IsNaN(normalized) ? 0f : Mathf.Clamp01(normalized);
        targetFill = normalized;
        elapsed = 0f;
        isAnimating = true;
    }
}