using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An order represents a single set of resources that a customer expects at the end of a specific week.
/// </summary>
public class Order
{
    /// <summary>
    /// The day that the order is due at the end.
    /// </summary>
    public int DueDay { get; private set; }

    /// <summary>
    /// The resources expected in the order.
    /// </summary>
    public ResourceCollection OrderedResources { get; private set; }

    public Order(int week, ResourceCollection orderedResources)
    {
        DueDay = week * Game.DAYS_PER_WEEK;
        OrderedResources = orderedResources;
    }
}
