using UnityEngine;
using TMPro;

public class UI_DatePanel : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI MonthText;
    public TextMeshProUGUI DateText;
    public TextMeshProUGUI TimeOfDayText;

    public void Refresh()
    {
        MonthText.text = Game.Instance.CurrentMonthName;
        DateText.text = $"{Game.Instance.GetWeekdayName()}, Week {Game.Instance.GetWeekNumber()}";
        TimeOfDayText.text = Game.Instance.GetCurrentTimeOfDay();
    }
}
