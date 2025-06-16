using UnityEngine;

public class CostBar : MonoBehaviour
{
    [SerializeField] private GameObject[] costObjects;
    private CostManager costManager;

    public void SetCost(int current, int max)
    {
        for (int i = 0; i < costObjects.Length; i++)
        {
            if (i < max)
                costObjects[i].SetActive(i < current);
            else
                costObjects[i].SetActive(false);
        }
    }

    public void Initialize(CostManager cm)
    {
        costManager = cm;
        costManager.OnCostChanged += SetCost;
        // ★ 최초 1회 강제 동기화
        SetCost(GetCurrentCost(), GetMaxCost());
    }

    // CostManager의 현재 값 읽기 (프로퍼티 추가 필요)
    private int GetCurrentCost() => costManager != null ? costManager.CurrentCost : 0;
    private int GetMaxCost() => costManager != null ? costManager.MaxCost : 0;
}