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
            Description = "Reliable and crisp. Grows underground, but always finds the light.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
                ObjectTagDefOf.Root,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "CompostHeap",
            Label = "compost heap",
            Tier = ObjectTierDefOf.Common,
            Description = "Old scraps, new life. Everything returns to the soil in time.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Fertilizer,
                ObjectTagDefOf.Compost,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fertility, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "CornStalk",
            Label = "corn stalk",
            Tier = ObjectTierDefOf.Common,
            Description = "Tall and golden, swaying in the breeze. A sign of steady growth.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "Mint",
            Label = "mint",
            Tier = ObjectTierDefOf.Common,
            Description = "A fresh, fragrant plant often used in teas and remedies.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Herb,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Herbs, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "Scarecrow",
            Label = "scarecrow",
            Tier = ObjectTierDefOf.Common,
            Description = "Stakes its ground, quietly convincing nearby soil to join the cause.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Marker,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Expansion, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "FirePit",
            Label = "fire pit",
            Tier = ObjectTierDefOf.Common,
            Description = "Burns wood into kindling but scorches the soil.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Combustion,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
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
            Description = "A wild flower with beautiful and colorful petals.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Flower,
                ObjectTagDefOf.Ornamental,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Ornaments, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "Flax",
            Label = "flax",
            Tier = ObjectTierDefOf.Common,
            Description = "A pale, wiry plant that’s more useful than it looks.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Flower,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fiber, 1 },
            }),
        },

        new ObjectDef()
        {
            DefName = "BeeHive",
            Label = "bee hive",
            Tier = ObjectTierDefOf.Rare,
            Description = "The workers go where the flowers grow.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Animal,
                ObjectTagDefOf.Pollinator,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
                { ResourceDefOf.Ornaments, 1 },
            }),
            Effects = new List<ObjectEffect>()
            {
                new AdjacencyEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        TagsAny = new List<ObjectTagDef>()
                        {
                            ObjectTagDefOf.Flower,
                        },
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        NativeProductionModifier = 1
                    },
                },
            },
        },

        new ObjectDef()
        {
            DefName = "WickerFrame",
            Label = "wicker frame",
            Tier = ObjectTierDefOf.Rare,
            Description = "Woven with care, it brings elegance to even the roughest fibers.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Crafting,
                ObjectTagDefOf.Ornamental,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Ornaments, 1 },
            }),
            Effects = new List<ObjectEffect>()
            {
                new SelfAdjacencyEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        NativeProductionAny = new List<ResourceDef>()
                        {
                            ResourceDefOf.Fiber,
                        },
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                        {
                            { ResourceDefOf.Ornaments, 1 },
                        },
                    },
                },
            },
        },

        new ObjectDef()
        {
            DefName = "CharcoalKiln",
            Label = "charcoal kiln",
            Tier = ObjectTierDefOf.Rare,
            Description = "Where smoke lingers, value rises. It burns low and gives much.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Combustion,
                ObjectTagDefOf.Crafting,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Kindle, 2 },
            }),
            Effects = new List<ObjectEffect>()
            {
                new SelfAdjacencyEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        NativeProductionAny = new List<ResourceDef>()
                        {
                            ResourceDefOf.Food,
                        },
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                        {
                            { ResourceDefOf.Kindle, 1 },
                        },
                    },
                },
            },
        },

        
        new ObjectDef()
        {
            DefName = "VerdantIdol",
            Label = "verdant idol",
            Tier = ObjectTierDefOf.Epic,
            Description = "Its roots twist deeper than the soil. Growth listens when it speaks.",
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Fertilizer,
                ObjectTagDefOf.Idol,
            },
            NativeProduction = new ResourceCollection(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 2 },
                { ResourceDefOf.Fertility, 1 },
            }),
            Effects = new List<ObjectEffect>()
            {
                new AdjacencyEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        TagsAny = new List<ObjectTagDef>()
                        {
                            ObjectTagDefOf.Plant,
                        }
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                        {
                            { ResourceDefOf.Food, 1 }
                        },
                        AppliedModifier = ObjectModifierDefOf.VerdantlyIdolized,
                    },
                },
                new SelfEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        TerrainAny = new List<TerrainDef>()
                        {
                            TerrainDefOf.FertileSoil,
                        },
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                        {
                            { ResourceDefOf.Food, 1 },
                            { ResourceDefOf.Ornaments, 1 },
                        }
                    },
                }
            },
        },
        
    };
}