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
            FlavorText = "Reliable and crisp. Grows underground, but always finds the light.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
                ObjectTagDefOf.Root,
                ObjectTagDefOf.FoodSource,
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
            FlavorText = "Old scraps, new life. Everything returns to the soil in time.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Fertilizer,
                ObjectTagDefOf.Compost,
            },
            Effects = new List<ObjectEffect>()
            {
                new AdjacencyEffect()
                {
                    AffectedTagsAll = new List<ObjectTagDef>()
                    {
                        ObjectTagDefOf.FoodSource,
                        ObjectTagDefOf.Plant,
                    },
                    ResourceProductionBonus = new Dictionary<ResourceDef, int>()
                    {
                        { ResourceDefOf.Food, 1 },
                    },
                },
            },
        },

        new ObjectDef()
        {
            DefName = "CornStalk",
            Label = "corn stalk",
            FlavorText = "Tall and golden, swaying in the breeze. A sign of steady growth.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
                ObjectTagDefOf.FoodSource
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            }),
            Effects = new List<ObjectEffect>()
            {
                new SelfAdjacencyEffect()
                {
                    EffectCriteria_TagsAny = new List<ObjectTagDef>()
                    {
                        ObjectTagDefOf.Plant,
                    },
                    ResourceProductionBonus = new Dictionary<ResourceDef, int>()
                    {
                        { ResourceDefOf.Food, 1 },
                    },
                },
            },
        },

        new ObjectDef()
        {
            DefName = "Mint",
            Label = "mint",
            FlavorText = "A fresh, fragrant plant often used in teas and remedies.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Herb,
                ObjectTagDefOf.FoodSource,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Herbs, 1 },
            }),
        },
    };
}