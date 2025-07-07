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
            Description = "A simple carrot",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
                ObjectTagDefOf.Root,
                ObjectTagDefOf.FoodSource,
            },
            BaseResources = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            },
        },

        new ObjectDef()
        {
            DefName = "CompostHeap",
            Label = "compost head",
            Description = "Enriches the soil. Adjacent food plants produce +1 food.",
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
                        { ResourceDefOf.Food, 1 }
                    }
                }
            }
        }
    };
}