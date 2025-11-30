using TMPro;
using UnityEngine;

public class GameStateView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI missText;
    [SerializeField] private TextMeshProUGUI timerText;

    public void UpdateRight(int value)
    {
        rightText.text = value.ToString();
    }

    public void UpdateMiss(int value)
    {
        missText.text = value.ToString();
    }

    public void UpdateTimer(int value)
    {
        timerText.text = $"{value / 60f:00}:{value % 60f:00}";
    }
}
