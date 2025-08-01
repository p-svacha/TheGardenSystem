using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An order represents a single set of resources that a customer expects at the end of a specific week.
/// </summary>
public class Order : IDraftable
{
    /// <summary>
    /// The customer that placed this order.
    /// </summary>
    public Customer Customer { get; protected set; }

    /// <summary>
    /// The day that the order is due at the end.
    /// </summary>
    public int DueDay { get; protected set; }

    /// <summary>
    /// The resources expected in the order.
    /// </summary>
    public ResourceCollection OrderedResources { get; protected set; }

    /// <summary>
    /// The resources that the player receives upon completion of this order.
    /// </summary>
    public ResourceCollection Reward { get; protected set; }

    public Order() { }
    public Order(Customer customer, int day, ResourceCollection orderedResources, ResourceCollection reward)
    {
        Customer = customer;
        DueDay = day;
        OrderedResources = orderedResources;
        Reward = reward;
    }

    public virtual string Label => Customer.LabelCapWord;
}
