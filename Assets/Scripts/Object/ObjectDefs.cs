using UnityEngine;
using System.Collections.Generic;

public static class ObjectDefs
{
    public static List<ObjectDef> Defs => new List<ObjectDef>()
    {
        new ObjectDef()
        {
            DefName = "Carrot",
            Label = "carrot",
            Tier = ObjectTierDefOf.Common,
            FlavorText = "Reliable and crisp. Grows underground, but always finds the light.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
                ObjectTagDefOf.Root,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "CompostHeap",
            Label = "compost heap",
            Tier = ObjectTierDefOf.Common,
            FlavorText = "Old scraps, new life. Everything returns to the soil in time.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Fertilizer,
                ObjectTagDefOf.Compost,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fertility, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "CornStalk",
            Label = "corn stalk",
            Tier = ObjectTierDefOf.Common,
            FlavorText = "Tall and golden, swaying in the breeze. A sign of steady growth.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "Mint",
            Label = "mint",
            Tier = ObjectTierDefOf.Common,
            FlavorText = "A fresh, fragrant plant often used in teas and remedies.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Herb,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Herbs, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "Scarecrow",
            Label = "scarecrow",
            Tier = ObjectTierDefOf.Common,
            FlavorText = "Stakes its ground, quietly convincing nearby soil to join the cause.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Marker,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Expansion, 1 },
            }),
        },
    };
}