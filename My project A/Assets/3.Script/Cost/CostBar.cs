using System;
using System.Collections.Generic;
using UnityEngine;

public class CostBar : MonoBehaviour
{
    [SerializeField] private List<GameObject> costIcons; // 8개의 방패 슬롯

    /// <summary>
    /// CostManager를 받아서 이벤트를 구독합니다.
    /// </summary>
    public void Initialize(CostManager mgr)
    {
        mgr.OnCostChanged += UpdateUI;
    }

    private void OnDestroy()
    {
        // 구독 해제 (메모리 누수 방지)
        var mgr = FindObjectOfType<CostManager>();
        if (mgr != null) mgr.OnCostChanged -= UpdateUI;
    }

    private void UpdateUI(int current, int max)
    {
        for (int i = 0; i < costIcons.Count; i++)
        {
            costIcons[i].SetActive(i < current);
        }
    }
}