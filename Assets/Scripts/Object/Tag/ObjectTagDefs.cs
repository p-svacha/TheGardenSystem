using UnityEngine;
using System.Collections.Generic;
public static class ObjectTagDefs
{
    public static List<ObjectTagDef> Defs => new List<ObjectTagDef>()
    {
        new ObjectTagDef()
        {
            DefName = "Plant",
            Label = "plant",
            Description = "A living, rooted organism that typically grows from soil and may produce food.",
            Color = new Color(0.40f, 0.67f, 0.37f),
        },

        new ObjectTagDef()
        {
            DefName = "Vegetable",
            Label = "vegetable",
            Description = "An edible plant part, usually cultivated for food.",
            Color = new Color(0.82f, 0.59f, 0.20f),
        },

        new ObjectTagDef()
        {
            DefName = "Root",
            Label = "root",
            Description = "A subterranean plant part used for anchoring and nutrient storage. Often edible.",
            Color = new Color(0.58f, 0.39f, 0.26f),
        },

        new ObjectTagDef()
        {
            DefName = "FoodSource",
            Label = "food source",
            Description = "An object that passively or actively generates food resources over time.",
            Color = new Color(0.93f, 0.71f, 0.23f),
        },

        new ObjectTagDef()
        {
            DefName = "Structure",
            Label = "structure",
            Description = "A built, non-living object that provides effects but does not grow or move.",
            Color = new Color(0.5f, 0.5f, 0.5f),
        },

        new ObjectTagDef()
        {
            DefName = "Fertilizer",
            Label = "fertilizer",
            Description = "An object or material that enhances the growth or productivity of nearby plants.",
            Color = new Color(0.36f, 0.45f, 0.23f),
        },

        new ObjectTagDef()
        {
            DefName = "Compost",
            Label = "compost",
            Description = "Decomposed organic matter that can enrich soil and support plant-based synergies.",
            Color = new Color(0.38f, 0.27f, 0.19f),
        },
    };
}
