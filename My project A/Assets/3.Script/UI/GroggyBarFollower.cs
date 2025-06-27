using UnityEngine;
using UnityEngine.UI;

public class GroggyBarFollower : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private Transform _target;
    private Vector3 _worldOffset;
    private RectTransform _rectTransform;
    private Camera _mainCam;

    // 🔽 상태 이상 스턴 아이콘
    [SerializeField] private GameObject stunIcon;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _mainCam = Camera.main;

        // 자동 연결 시도 (한 번만)
        if (stunIcon == null)
        {
            Transform t = transform.Find("stun");
            if (t != null)
                stunIcon = t.gameObject;
        }
    }

    void LateUpdate()
    {
        if (_target == null || _mainCam == null) return;

        Vector3 screenPos = _mainCam.WorldToScreenPoint(_target.position + _worldOffset);
        _rectTransform.position = screenPos;
    }

    public void Initialize(Transform target, Vector3 offset)
    {
        _target = target;
        _worldOffset = offset;
    }

    public void SetGroggy(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);
        fillImage.fillAmount = float.IsNaN(normalized) ? 1 : normalized;

        // 🟡 스턴 아이콘 처리: Groggy가 0이면 활성화, 아니면 비활성화
        if (stunIcon != null)
            stunIcon.SetActive(normalized <= 0f);
    }
}