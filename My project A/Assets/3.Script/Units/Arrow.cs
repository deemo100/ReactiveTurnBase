using UnityEngine;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float height = 2f;
    private Vector3 start;
    private Vector3 end;
    private float time;
    private float duration = 0.3f;
    private bool isFlying = false;

    public void SetTarget(Vector3 target, float flyTime)
    {
        start = transform.position;
        end = target;
        time = 0;
        duration = flyTime;
        isFlying = true;
    }

    void Update()
    {
        if (!isFlying) return;

        Vector3 prevPos = transform.position;

        time += Time.deltaTime / duration;
        if (time >= 1.0f)
        {
            transform.position = end;
            Destroy(gameObject, 0.05f);
            isFlying = false;
            return;
        }

        Vector3 pos = Vector3.Lerp(start, end, time);
        pos.y += Mathf.Sin(Mathf.PI * time) * height;

        // **방향 회전**
        Vector3 moveDir = (pos - prevPos).normalized;
        if (moveDir.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 45f, Vector3.forward); // ← +90도 보정!
        }

        transform.position = pos;
    }
}
