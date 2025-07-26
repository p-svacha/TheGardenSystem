using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TerrainDefs
{
    public static List<TerrainDef> Defs => new List<TerrainDef>()
    {
        new TerrainDef()
        {
            DefName = "Soil",
            Label = "soil",
            Description = "Soil, good for plants to grow on.",
            BetterFertilityTerrainDefName = "FertileSoil",
            WorseFertilityTerrainDefName = "BarrenSoil",
        },

        new TerrainDef()
        {
            DefName = "FertileSoil",
            Label = "fertile soil",
            Description = "Fertile soil, increasing the production of plants on it.",
            WorseFertilityTerrainDefName = "Soil",
            Effects = new List<ObjectEffect>()
            {
                new TerrainEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        TagsAny = new List<ObjectTagDef>()
                        {
                            ObjectTagDefOf.Plant,
                        },
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        NativeProductionModifier = 1,
                    },
                },
            },
        },

        new TerrainDef()
        {
            DefName = "BarrenSoil",
            Label = "barren soil",
            Description = "Soil stripped of vitality. Plants struggle to grow here, yielding less than usual.",
            BetterFertilityTerrainDefName = "Soil",
            Effects = new List<ObjectEffect>()
            {
                new TerrainEffect()
                {
                    EffectCriteria = new EffectCriteria()
                    {
                        TagsAny = new List<ObjectTagDef>()
                        {
                            ObjectTagDefOf.Plant,
                        },
                    },
                    EffectOutcome = new EffectOutcome()
                    {
                        NativeProductionModifier = -1,
                    },
                },
            },
        },
    };
}
