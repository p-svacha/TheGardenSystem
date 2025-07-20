using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_OrderPanel : MonoBehaviour
{
    [Header("Elements")]
    public GameObject ChapterContainer;

    [Header("Prefabs")]
    public UI_OrderChapter OrderGroupPrefab;

    public void Refresh()
    {
        List<Order> allActiveOrders = Game.Instance.ActiveOrders;

        // Group orders by due date
        var orderGroups = allActiveOrders.GroupBy(o => o.DueDay);

        // Create chapters for each group
        HelperFunctions.DestroyAllChildredImmediately(ChapterContainer, skipElements: 1);
        foreach (var orderGroup in orderGroups)
        {
            UI_OrderChapter elem = GameObject.Instantiate(OrderGroupPrefab, ChapterContainer.transform);
            elem.Init(orderGroup.Key, orderGroup.ToList());
        }
    }
}
