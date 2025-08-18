using UnityEngine;
using System.Collections.Generic;

public static class ObjectDefs
{
    public static List<ObjectDef> Defs => new List<ObjectDef>()
    {
        new ObjectDef()
        {
            DefName = "Shed",
            Label = "shed",
            Description = "An old wooden shed that marks a garden sector and holds its inventory. Click to view or manage items stored in this sector. Has no effects or resource production.",
            Tier = ObjectTierDefOf.Special,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Permanent,
            },
        },

        new ObjectDef()
        {
            DefName = "Carrot",
            Label = "carrot",
            Description = "Reliable and crisp. Grows underground, but always finds the light.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
                ObjectTagDefOf.Root,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            },
        },

        new ObjectDef()
        {
            DefName = "Worm",
            Label = "worm",
            Description = "A smal wriggly rain worm that is healthy for roots and the soil in general.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Animal,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fertility, 1 },
            },
            Effects = new List<ObjectEffect>()
            {
                new AdjacencyEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        TagsAll = new List<ObjectTagDef>()
                        {
                            ObjectTagDefOf.Root
                        }
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        NativeProductionModifier = 1
                    },
                },
            }
        },

        new ObjectDef()
        {
            DefName = "CompostHeap",
            Label = "compost heap",
            Description = "Old scraps, new life. Everything returns to the soil in time.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Fertilizer,
                ObjectTagDefOf.Compost,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fertility, 2 },
            },
        },

        new ObjectDef()
        {
            DefName = "Corn",
            Label = "corn",
            Description = "A single golden knob of corn.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Vegetable,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            },
        },

        new ObjectDef()
        {
            DefName = "Mint",
            Label = "mint",
            Description = "A fresh, fragrant plant often used in teas and remedies.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Herb,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Herbs, 1 },
            },
        },

        new ObjectDef()
        {
            DefName = "FirePit",
            Label = "fire pit",
            Description = "Burns wood into kindling but scorches the soil.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Combustion,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Kindle, 2 },
                { ResourceDefOf.Fertility, -1 },
            },
        },

        new ObjectDef()
        {
            DefName = "Daisy",
            Label = "daisy",
            Description = "A wild flower that can be used for decoration.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Flower,
                ObjectTagDefOf.Ornamental,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Decorations, 1 },
            },
        },

        new ObjectDef()
        {
            DefName = "Flax",
            Label = "flax",
            Description = "A pale, wiry plant that’s more useful than it looks.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Plant,
                ObjectTagDefOf.Flower,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fiber, 1 },
            },
        },

        new ObjectDef()
        {
            DefName = "BeeHive",
            Label = "bee hive",
            Description = "The workers go where the flowers grow.",
            Tier = ObjectTierDefOf.Rare,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Animal,
                ObjectTagDefOf.Pollinator,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
                { ResourceDefOf.Decorations, 1 },
            },
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
            Description = "Woven with care, it brings elegance to even the roughest fibers.",
            Tier = ObjectTierDefOf.Rare,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Crafting,
                ObjectTagDefOf.Ornamental,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Decorations, 1 },
            },
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
                            { ResourceDefOf.Decorations, 1 },
                        },
                    },
                },
            },
        },

        new ObjectDef()
        {
            DefName = "CharcoalKiln",
            Label = "charcoal kiln",
            Description = "Where smoke lingers, value rises. It burns low and gives much.",
            Tier = ObjectTierDefOf.Rare,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Combustion,
                ObjectTagDefOf.Crafting,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Kindle, 2 },
            },
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
            Description = "Its roots twist deeper than the soil. Growth listens when it speaks.",
            Tier = ObjectTierDefOf.Epic,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Fertilizer,
                ObjectTagDefOf.Idol,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 2 },
                { ResourceDefOf.Fertility, 1 },
            },
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
                        AppliedObjectModifier = ModifierDefOf.VerdantlyIdolized,
                        AppliedObjectModifierDuration = 3,
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
                            { ResourceDefOf.Decorations, 1 },
                        }
                    },
                }
            },
        },
        
        
        new ObjectDef()
        {
            DefName = "EmberCore",
            Label = "ember core",
            Description = "Still hot from wherever it came from. Burn carefully, or burn everything.",
            Tier = ObjectTierDefOf.Epic,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Combustion,
                ObjectTagDefOf.Hazard,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Kindle, 4 },
                { ResourceDefOf.Fertility, -2 },
            },
            Effects = new List<ObjectEffect>()
            {
                new AdjacencyEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        TagsAny = new List<ObjectTagDef>()
                        {
                            ObjectTagDefOf.Combustion,
                        },
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                        {
                            { ResourceDefOf.Kindle, 2 },
                        }
                    },
                },
                new SelfEffect()
                {
                    EffectOutcome = new EffectOutcome()
                    {
                        AppliedTileModifier = ModifierDefOf.Scorched,
                        AppliedTileModifierDuration = 3,
                    }
                }
            }
        },
        
        new ObjectDef()
        {
            DefName = "BloomingPavillon",
            Label = "blooming pavillon",
            Description = "Built for nothing but beauty.",
            Tier = ObjectTierDefOf.Epic,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Structure,
                ObjectTagDefOf.Ornamental,
                ObjectTagDefOf.Prestige,
                ObjectTagDefOf.Luxury,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Decorations, 2 },
                { ResourceDefOf.Herbs, 1 },
            },
            Effects = new List<ObjectEffect>()
            {
                new AdjacencyEffect()
                {
                    Radius = 2,
                    EffectCriteria = new EffectCriteria()
                    {
                        TagsAny = new List<ObjectTagDef>()
                        {
                            ObjectTagDefOf.Ornamental,
                        },
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                        {
                            { ResourceDefOf.Decorations, 1 },
                        }
                    },
                },
                new SelfEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        TagNearby = ObjectTagDefOf.Ornamental,
                        TagNearbyThreshhold = 3,
                        TagNearbyRadius = 2,
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                        {
                            { ResourceDefOf.Decorations, 3 },
                        }
                    },
                },
            },
        },

        new ObjectDef()
        {
            DefName = "BronzeCoin",
            Label = "bronze coin",
            Description = "An old coin that's not worth a lot, but better than nothing.",
            Tier = ObjectTierDefOf.Common,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Metal,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Gold, 1 },
            }
        },

        new ObjectDef()
        {
            DefName = "SilverCoin",
            Label = "silver coin",
            Description = "A valuable coin providing some income.",
            Tier = ObjectTierDefOf.Rare,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Metal,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Gold, 2 },
            }
        },

        new ObjectDef()
        {
            DefName = "GoldCoin",
            Label = "gold coin",
            Description = "A precious coin worth a lot.",
            Tier = ObjectTierDefOf.Epic,
            Tags = new List<ObjectTagDef>()
            {
                ObjectTagDefOf.Metal,
            },
            NativeProduction = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Gold, 3 },
            }
        },
    };
}