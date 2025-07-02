using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        var allButtons = FindObjectsOfType<StageButton>();
        foreach (var btn in allButtons)
        {
            btn.RefreshStarUI();
        }
    }
}