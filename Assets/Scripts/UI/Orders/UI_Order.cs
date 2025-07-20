using UnityEngine;
using TMPro;

public class UI_Order : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI CustomerText;
    public TextMeshProUGUI ResourceText;

    public void Init(Order order)
    {
        CustomerText.text = "";
        ResourceText.text = order.OrderedResources.GetAsSingleLinkedString();
    }
}
