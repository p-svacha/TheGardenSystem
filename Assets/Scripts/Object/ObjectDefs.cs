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

        new ObjectDef()
        {
            DefName = "FirePit",
            Label = "fire pit",
            Tier = ObjectTierDefOf.Common,
            FlavorText = "Burns wood into kindling but scorches the soil.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Combustion,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Kindle, 2 },
                { ResourceDefOf.Fertility, -1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "Wildflower",
            Label = "wildflower",
            Tier = ObjectTierDefOf.Common,
            FlavorText = "A wild flower with beautiful and colorful petals.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Flower,
                ObjectTagDefOf.Ornamental,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Ornaments, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "Flax",
            Label = "flax",
            Tier = ObjectTierDefOf.Common,
            FlavorText = "A pale, wiry plant that’s more useful than it looks.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Flower,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fiber, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "BeeHive",
            Label = "bee hive",
            Tier = ObjectTierDefOf.Rare,
            FlavorText = "The workers go where the flowers grow.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Animal,
                ObjectTagDefOf.Pollinator,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
                { ResourceDefOf.Ornaments, 1 },
            }),
            Effects = new List<ObjectEffect>()
            {
                new AdjacencyEffect()
                {
                    EffectCriteria_TagsAny = new List<ObjectTagDef>()
                    {
                        ObjectTagDefOf.Flower,
                    },
                    NativeProductionModifier = 1
                },
            },
        },

        new ObjectDef()
        {
            DefName = "WickerFrame",
            Label = "wicker frame",
            Tier = ObjectTierDefOf.Rare,
            FlavorText = "Woven with care, it brings elegance to even the roughest fibers.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Crafting,
                ObjectTagDefOf.Ornamental,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Ornaments, 1 },
            }),
            Effects = new List<ObjectEffect>()
            {
                new SelfAdjacencyEffect()
                {
                    EffectCriteria_NativeProduction_Any = new List<ResourceDef>()
                    {
                        ResourceDefOf.Fiber,
                    },
                    ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                    {
                        { ResourceDefOf.Ornaments, 1 },
                    },
                },
            },
        },

        new ObjectDef()
        {
            DefName = "CharcoalKiln",
            Label = "charcoal kiln",
            Tier = ObjectTierDefOf.Rare,
            FlavorText = "Where smoke lingers, value rises. It burns low and gives much.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Combustion,
                ObjectTagDefOf.Crafting,
            },
            BaseResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Kindle, 2 },
            }),
            Effects = new List<ObjectEffect>()
            {
                new SelfAdjacencyEffect()
                {
                    EffectCriteria_NativeProduction_Any = new List<ResourceDef>()
                    {
                        ResourceDefOf.Food,
                    },
                    ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                    {
                        { ResourceDefOf.Kindle, 1 },
                    },
                },
            },
        },
    };
}