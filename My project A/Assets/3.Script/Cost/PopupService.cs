using System.Collections;     // IEnumerator, WaitForSeconds
using UnityEngine;
using TMPro;

public class PopupService : MonoBehaviour, IPopupService
{
    [SerializeField] private TMP_Text     _popupText;
    [SerializeField] private CanvasGroup  _group;
    [SerializeField] private float        _duration = 1.5f;

    public void Show(string message)
    {
        StopAllCoroutines();
        StartCoroutine(DoPopup(message));
    }

    private IEnumerator DoPopup(string msg)
    {
        _popupText.text = msg;
        _group.alpha   = 1f;
        yield return new WaitForSeconds(_duration);
        _group.alpha   = 0f;
    }
}