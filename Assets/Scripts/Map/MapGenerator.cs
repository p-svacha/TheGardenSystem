using UnityEngine;

public static class MapGenerator
{
    public static Map GenerateMap(int size)
    {
        MapTile[,] tiles = new MapTile[size,size];
        Map map = new Map();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                tiles[x, y] = new MapTile(map, coordinates, TerrainDefOf.Soil);
            }
        }

        map.Initialize(tiles);
        return map;
    }
}
