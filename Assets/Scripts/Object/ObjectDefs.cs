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
    };
}