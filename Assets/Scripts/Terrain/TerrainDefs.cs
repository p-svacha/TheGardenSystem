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
            Sprite = ResourceManager.LoadSprite("Sprites/Terrain/Soil")
        }
    };
}
