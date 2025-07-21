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
        },

        new TerrainDef()
        {
            DefName = "FertileSoil",
            Label = "fertile soil",
            Description = "Fertile soil, increasing the production of plants on it",
            WorseFertilityTerrainDefName = "Soil",
        },
    };
}
