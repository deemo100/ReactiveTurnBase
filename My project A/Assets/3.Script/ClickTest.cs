using UnityEngine;
using UnityEngine.InputSystem;

public class ClickTest : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Debug.Log("마우스 좌클릭: " + mousePos);
        }
    }
}