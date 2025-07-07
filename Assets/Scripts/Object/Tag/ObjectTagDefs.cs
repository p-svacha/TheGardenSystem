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
        },

        new ObjectTagDef()
        {
            DefName = "Vegetable",
            Label = "vegetable",
            Description = "An edible plant part, usually cultivated for food.",
        },

        new ObjectTagDef()
        {
            DefName = "Root",
            Label = "root",
            Description = "A subterranean plant part used for anchoring and nutrient storage. Often edible."
        },

        new ObjectTagDef()
        {
            DefName = "FoodSource",
            Label = "food source",
            Description = "An object that passively or actively generates food resources over time.",
        },

        new ObjectTagDef()
        {
            DefName = "Structure",
            Label = "structure",
            Description = "A built, non-living object that provides effects but does not grow or move.",
        },

        new ObjectTagDef()
        {
            DefName = "Fertilizer",
            Label = "fertilizer",
            Description = "An object or material that enhances the growth or productivity of nearby plants."
        },

        new ObjectTagDef()
        {
            DefName = "Compost",
            Label = "compost",
            Description = "Decomposed organic matter that can enrich soil and support plant-based synergies."
        },
    };
}
