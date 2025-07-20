using System.Collections.Generic;
using UnityEngine;

public static class CustomerDefs
{
    public static List<CustomerDef> Defs => new List<CustomerDef>()
    {
        new CustomerDef()
        {
            DefName = "TownCanteen",
            Label = "town canteen",
            Description = "The local canteen serving affordable lunches to public workers.",
            Backstory = "Nestled behind the town hall, the Town Canteen has been around for decades, feeding bus drivers, clerks, and school staff with no-frills, hearty meals. It’s publicly funded and always on the lookout for reliable suppliers who can offer basic produce in steady quantities. Your garden caught their eye thanks to your note on the community board.",
            Orders = new List<ResourceCollection>()
            {
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 5 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 7 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 10 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 14 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 19 } }) }, // 5
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 25 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 32 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 40 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 49 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 59 } }) }, // 10
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 70 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 82 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 95 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 109 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 124 } }) }, // 15
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 140 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 157 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 185 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 204 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 224 } }) }, // 20
            },
        }
    };
}
