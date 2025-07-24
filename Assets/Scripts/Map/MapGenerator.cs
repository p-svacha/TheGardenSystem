using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public static Map GenerateMap(int size)
    {
        if (size % 2 != 1) throw new System.Exception("Size needs to be an odd number");

        Dictionary<Vector2Int, MapTile> tiles = new Dictionary<Vector2Int, MapTile>();
        Map map = new Map();
        int half = size / 2;

        for (int x = -half; x <= half; x++)
        {
            for (int y = -half; y <= half; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);

                TerrainDef terrain = TerrainDefOf.Soil;

                float distanceToCenter = coordinates.magnitude;
                bool inStartingArea = (distanceToCenter <= 2);
                if(!inStartingArea)
                {
                    float barrenSoilChance = 0.05f;
                    float fertileSoilChance = distanceToCenter * 0.06f;
                    if (Random.value < fertileSoilChance) terrain = TerrainDefOf.FertileSoil;
                    else if (Random.value < barrenSoilChance) terrain = TerrainDefOf.BarrenSoil;
                }

                tiles.Add(coordinates, new MapTile(map, coordinates, terrain));
            }
        }

        map.Initialize(tiles);
        return map;
    }
}
