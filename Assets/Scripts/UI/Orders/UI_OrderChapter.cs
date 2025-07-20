using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UI_OrderChapter : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI DueText;
    public TextMeshProUGUI TotalResourcesText;
    public GameObject OrdersContainer;

    [Header("Prefabs")]
    public UI_Order OrderPrefab;

    public void Init(int dueDay, List<Order> orders)
    {
        // Calculate total resources
        ResourceCollection totalRes = new ResourceCollection();
        foreach (Order order in orders) totalRes.AddResources(order.OrderedResources);

        // Display summary
        int daysRemanining = dueDay - Game.Instance.Day + 1;
        DueText.text = $"Due in {daysRemanining} days";
        if (daysRemanining == 1) DueText.text = "Due today";
        TotalResourcesText.text = totalRes.GetAsSingleLinkedString();

        // Individual orders
        HelperFunctions.DestroyAllChildredImmediately(OrdersContainer, skipElements: 1);
        foreach(Order order in orders)
        {
            UI_Order elem = GameObject.Instantiate(OrderPrefab, OrdersContainer.transform);
            elem.Init(order);
        }
    }
}
