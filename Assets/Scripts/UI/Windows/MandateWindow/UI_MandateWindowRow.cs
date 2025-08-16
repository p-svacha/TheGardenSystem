using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MandateWindowRow : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI MonthText;
    public TextMeshProUGUI ResourceText;
    public Image DeliveredIcon;

    public void Init(TownMandate mandate, bool isCompleted)
    {
        MonthText.text = $"<b>{Game.Instance.GetMonthName(mandate.MonthIndex)}</b> - {mandate.Def.Label}";
        ResourceText.text = mandate.OrderedResources.GetAsSingleLinkedString();
        DeliveredIcon.gameObject.SetActive(isCompleted);
    }
}
