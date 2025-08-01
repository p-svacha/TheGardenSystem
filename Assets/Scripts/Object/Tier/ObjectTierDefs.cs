using System.Collections.Generic;
using UnityEngine;

public static class ObjectTierDefs
{
    public static List<ObjectTierDef> Defs => new List<ObjectTierDef>()
    {
        new ObjectTierDef()
        {
            DefName = "Common",
            Label = "common",
            Description = "Most common objects that are drafted at the end of each week.",
            MarketValue = 10,
            // Power level guideline: 100%
        },

        new ObjectTierDef()
        {
            DefName = "Rare",
            Label = "rare",
            Description = "Powerful rare objects that are harder to acquire. A Rare object is drafted at the end of each week.",
            MarketValue = 20,
            // Power level guideline: 200% 
        },

        new ObjectTierDef()
        {
            DefName = "Epic",
            Label = "epic",
            Description = "Very powerful but rare objects. An epic object is drafted at the end of each month (every 4th week).",
            MarketValue = 40,
            // Power level guideline: 400% 
        },

        new ObjectTierDef()
        {
            DefName = "Legendary",
            Label = "legendary",
            Description = "The most powerful objects. Legendary objects cannot be acquired through drafting.",
            MarketValue = 80,
            // Power level guideline: 800% 
        },

        new ObjectTierDef()
        {
            DefName = "Cursed",
            Label = "cursed",
            Description = "Objects with strictly negative effects, that are added as punishments or difficulty Modifiers. Cursed objects cannot be acquired through drafting.",
            // Power level guideline: Variable, always negative
        },

        new ObjectTierDef()
        {
            DefName = "Special",
            Label = "special",
            Description = "Extraordinary objects with extraordinary origin, effects and purposes. Special objects cannot be acquired through drafting.",
            MarketValue = 30,
            // Power level guideline: Variable, depending on object
        },
    };
}
