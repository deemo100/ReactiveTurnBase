using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollower : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private Transform _target;
    private Vector3 _worldOffset;
    private RectTransform _rectTransform;
    private Camera _mainCam;

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
    }

    public void SetHealth(float normalized)
    {
        if (fillImage != null)
            fillImage.fillAmount = Mathf.Clamp01(normalized);
    }
}