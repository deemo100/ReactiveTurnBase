using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnitClickDetector : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                var unit = hit.collider.GetComponent<PlayerUnit>();
                if (unit != null)
                {
                    Debug.Log("[Raycast] 클릭된 PlayerUnit: " + unit.UnitName);
                    // 선택 처리 등 추가
                }
            }
        }
    }
}