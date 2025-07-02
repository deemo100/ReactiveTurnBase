using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryStarController : MonoBehaviour
{
    [Header("별 이미지 3개")]
    public Image[] stars;
    [Header("조건 텍스트 3개")]
    public TMP_Text[] conditionTexts;

    [Header("별/텍스트 색상")]
    public Color starActiveColor = Color.white;
    public Color starInactiveColor = Color.black;
    public Color textActiveColor = Color.white;
    public Color textInactiveColor = new Color(0.6f,0.6f,0.6f,1f);

    public void SetStars(bool cond1, bool cond2, bool cond3)
    {
        bool[] conds = new bool[] { cond1, cond2, cond3 };

        // 1. 달성한 조건 개수 카운트
        int achievedCount = 0;
        foreach (bool cond in conds)
            if (cond) achievedCount++;

        // 2. 별은 달성 개수만큼 왼쪽부터 활성
        for (int i = 0; i < stars.Length; i++)
            stars[i].color = (i < achievedCount) ? starActiveColor : starInactiveColor;

        // 3. 텍스트는 개별 조건에 따라 밝기/회색
        for (int i = 0; i < conditionTexts.Length; i++)
            conditionTexts[i].color = conds[i] ? textActiveColor : textInactiveColor;
    }
    public void SetStarsByCount(int starCount)
    {
        for (int i = 0; i < stars.Length; i++)
            stars[i].color = (i < starCount) ? starActiveColor : starInactiveColor;
    }
    
}