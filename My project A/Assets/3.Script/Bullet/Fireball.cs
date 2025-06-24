using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector3 start;
    private Vector3 end;
    private float duration = 0.3f; // 이동시간(초)
    private float time = 0f;
    private bool isFlying = false;

    public void SetTarget(Vector3 target, float flyTime = 0.3f)
    {
        start = transform.position;
        end = target;
        duration = flyTime;
        time = 0f;
        isFlying = true;

        // 방향 회전(스프라이트가 위쪽을 볼 경우 +90)
        Vector3 dir = (end - start).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        if (!isFlying) return;

        time += Time.deltaTime / duration;
        if (time >= 1.0f)
        {
            transform.position = end;
            Destroy(gameObject, 0.05f);
            isFlying = false;
            return;
        }
        // 직선 보간
        Vector3 pos = Vector3.Lerp(start, end, time);
        transform.position = pos;
    }
}