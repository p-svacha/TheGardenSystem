using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Customer
{
    public CustomerDef Def { get; private set; }
    public int OrderLevel { get; private set; }

    /// <summary>
    /// How many orders to this customer have been missed in a row.
    /// </summary>
    public int MissedOrders { get; private set; }

    public Customer(CustomerDef def, int orderLevel)
    {
        Def = def;
        OrderLevel = orderLevel;
    }

    public void IncreaseLevel()
    {
        OrderLevel++;
    }

    public void ResetMissedOrders() => MissedOrders = 0;
    public void IncrementMissedOrders() => MissedOrders++;

    public ResourceCollection GetCurrentLevelOrder()
    {
        // Start with an empty collection
        var total = new ResourceCollection();

        // Pre-extract sorted keys so we can quickly find the right step
        var keys = Def.OrderIncreases.Keys.OrderBy(k => k).ToList();

        for (int lvl = 1; lvl <= OrderLevel; lvl++)
        {
            // Find highest threshold <= lvl
            int applicableKey = keys.Last(k => k <= lvl);

            // Clone the increment for that key
            var increment = new ResourceCollection(Def.OrderIncreases[applicableKey]);

            // Add to total
            total.AddResources(increment);
        }

        return total;
    }

    public ResourceCollection GetCurrentLevelReward()
    {
        return new ResourceCollection(new Dictionary<ResourceDef, int>()
        {
            { ResourceDefOf.Gold, Def.GoldRewardPerLevel * OrderLevel }
        });
    }

    public string Label => Def.Label;
    public string LabelCap => Def.LabelCap;
    public string LabelCapWord => Def.LabelCapWord;
    public string Description => Def.Description;
    public string Backstory => Def.Backstory;
}
