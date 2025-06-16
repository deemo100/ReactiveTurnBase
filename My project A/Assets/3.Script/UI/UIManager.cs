using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TMP_Text turnText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    void Awake()
    {
        Instance = this;
        victoryPanel.SetActive(false);
        defeatPanel .SetActive(false);
    }

    public void UpdateTurnText(int turn)
    {
        turnText.text = $"TURN {turn}";
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
    }

    public void ShowDefeat()
    {
        defeatPanel.SetActive(true);
    }
}