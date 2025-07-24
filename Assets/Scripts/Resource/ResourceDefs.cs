using System.Collections.Generic;
using UnityEngine;

public static class ResourceDefs
{
    public static List<ResourceDef> Defs => new List<ResourceDef>()
    {
        // Market resources
        new ResourceDef()
        {
            DefName = "Food",
            Label = "food",
            Description = "Basic edible output from plants and animals. Most customers request food regularly.",
            Type = ResourceType.MarketResource,
        },

        new ResourceDef()
        {
            DefName = "Herbs",
            Label = "herbs",
            Description = "Fragrant or medicinal plants used in kitchens, clinics, and cosmetics. A niche demand, but valued by certain customers.",
            Type = ResourceType.MarketResource,
        },

        new ResourceDef()
        {
            DefName = "Ornaments",
            Label = "ornaments",
            Description = "Visually pleasing or fragrant goods used to beautify spaces. Includes flowers, decorative plants, and handmade items with aesthetic value.",
            Type = ResourceType.MarketResource,
        },

        new ResourceDef()
        {
            DefName = "Kindle",
            Label = "kindle",
            Description = "Dry, flammable materials gathered for fires and warmth. Used by townsfolk for cooking, heating, and rituals.",
            Type = ResourceType.MarketResource,
        },

        new ResourceDef()
        {
            DefName = "Fiber",
            Label = "fiber",
            Description = "Natural strands used in weaving, binding, and crafting. Essential for cloth, ropes, and light construction.",
            Type = ResourceType.MarketResource,
        },

        // Abstract resources
        new ResourceDef()
        {
            DefName = "Fertility",
            Label = "fertility",
            Description = "Enriches or depletes the soil fertility the object lands on. Terrain may change at certain fertility threshholds.",
            Type = ResourceType.AbstractResource,
        },

        new ResourceDef()
        {
            DefName = "Expansion",
            Label = "expansion",
            Description = $"Increases the claim of all 4-way adjacent unowned tiles. When a tile reaches {Game.CLAIMS_NEEDED_TO_ACQUIRE_TILES} claim, it gets added to your garden.",
            Type = ResourceType.AbstractResource,
        },
    };
}
