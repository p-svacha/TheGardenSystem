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
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 10 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 20 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 30 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 40 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 50 } }) }, // 5
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 60 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 70 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 80 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 90 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 100 } }) }, // 10
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 110 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 120 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 130 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 140 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 150 } }) }, // 15
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 160 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 170 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 180 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 190 } }) },
                { new (new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 200    } }) }, // 20
            },
        }
    };
}
