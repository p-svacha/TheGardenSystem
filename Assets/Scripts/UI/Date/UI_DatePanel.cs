using UnityEngine;
using TMPro;

public class UI_DatePanel : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI DateText;

    public void Refresh()
    {
        DateText.text = $"{Game.Instance.GetWeekdayName()}, Week {Game.Instance.GetWeekNumber()}";
    }
}
