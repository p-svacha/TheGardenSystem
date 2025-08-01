using System.Collections.Generic;
using UnityEngine;

public class CustomerDef : Def
{
    /// <summary>
    /// A very short, flavorful background of this customer.
    /// </summary>
    public string Backstory { get; init; }

    /// <summary>
    /// Flag if this customer can place weekly orders.
    /// If false, they will not appear as regular customers and only in specific circumstances.
    /// </summary>
    public bool IsWeeklyCustomer { get; init; } = true;

    /// <summary>
    /// The resources that this customer orders.
    /// The dictionary stores how much the ordered resource amount increases per level, whereas the level is the key of the dictionary.
    /// If there is no key for a specific order level, the increase is the same as the previous most defined key.
    /// 
    /// So for example in { 
    ///     {1, new(new Dictionary<ResourceDef, int> { {ResourceDefOf.Food, 10} }) }, 
    ///     {10, new(new Dictionary<ResourceDef, int> { {ResourceDefOf.Food, 20}, {ResourceDefOf.Herbs, 10} }) }, 
    /// }
    /// The ordered resources increase by 10 food per level from levels 1-9, and then by 20 food and 10 herbs per level at level 10+.
    /// </summary>
    public Dictionary<int, ResourceCollection> OrderIncreases { get; init; }

    /// <summary>
    /// The amount of gold the player receives for completing an order per order level.
    /// </summary>
    public int GoldRewardPerLevel { get; init; }

    public override bool Validate()
    {
        if (IsWeeklyCustomer)
        {
            if (!OrderIncreases.ContainsKey(1)) throw new System.Exception($"OrderIncreases in CustomerDef {DefName} must have something defined for Level 1.");
        }

        return base.Validate();
    }
}
