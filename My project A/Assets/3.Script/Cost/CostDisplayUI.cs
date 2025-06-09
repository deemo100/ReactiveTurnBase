using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Transform))]
public class CostDisplayUI : MonoBehaviour
{
    [SerializeField] private List<Image> _costIcons; // CostSet 밑 8 개 Image

    public void UpdateDisplay(int currentCost)
    {
        for (int i = 0; i < _costIcons.Count; i++)
        {
            _costIcons[i].color = i < currentCost
                ? Color.white      // 활성
                : Color.grey;      // 비활성
        }
    }
}