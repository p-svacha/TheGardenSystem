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
                tiles.Add(coordinates, new MapTile(map, coordinates, TerrainDefOf.Soil));
            }
        }

        map.Initialize(tiles);
        return map;
    }
}
