// Scripts/Input/PlayerClickTargetSelector.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]
public class PlayerClickTargetSelector : MonoBehaviour
{
    public UnityEvent<PlayerUnit> OnPlayerClicked;
    public UnityEvent<EnemyUnit>  OnEnemyClicked;

    Camera _cam;

    void Awake() => _cam = GetComponent<Camera>();

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) 
            return;

        Vector3 mp = Input.mousePosition;
        Debug.Log($"[Selector] MouseDown at {mp}");        

        Ray ray = _cam.ScreenPointToRay(mp);
        Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, 2f);

        if (Physics.Raycast(ray, out var hit, 100f))
        {
            Debug.Log($"[Selector] Ray hit: {hit.collider.name} (layer {LayerMask.LayerToName(hit.collider.gameObject.layer)})");

            if (hit.collider.TryGetComponent<PlayerUnit>(out var pu))
            {
                Debug.Log($"[Selector] Detected PlayerUnit: {pu.name}");
                OnPlayerClicked?.Invoke(pu);
            }
            else if (hit.collider.TryGetComponent<EnemyUnit>(out var eu))
            {
                Debug.Log($"[Selector] Detected EnemyUnit: {eu.name}");
                OnEnemyClicked?.Invoke(eu);
            }
            else
            {
                Debug.Log($"[Selector] Hit something else: {hit.collider.gameObject.name}");
            }
        }
        else
        {
            Debug.Log("[Selector] Ray hit nothing");
        }
    }
}