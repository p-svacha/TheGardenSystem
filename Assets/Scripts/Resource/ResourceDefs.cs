using System.Collections.Generic;
using UnityEngine;

public static class ResourceDefs
{
    public static List<ResourceDef> Defs => new List<ResourceDef>()
    {
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
        }
    };
}
