using UnityEngine;
using TMPro;

public class UI_Order : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI CustomerText;
    public TextMeshProUGUI ResourceText;

    public void Init(Order order)
    {
        CustomerText.text = $"{order.Customer.LabelCapWord} (Level {order.Customer.OrderLevel})";
        ResourceText.text = order.OrderedResources.GetAsSingleLinkedString();
    }
}
