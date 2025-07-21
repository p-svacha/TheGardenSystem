using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Map
{
    private MapTile[,] Tiles;

    public void Initialize(MapTile[,] tiles)
    {
        Tiles = tiles;
    }

    public void ClearAllObjects()
    {
        foreach (MapTile tile in AllTiles) tile.ClearObject();
    }

    public MapTile GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return null;
        return Tiles[x, y];
    }

    public void SetTerrain(Vector2Int coordinates, TerrainDef def)
    {
        Tiles[coordinates.x, coordinates.y].SetTerrain(def);
    }

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);
    public MapTile GetTile(Vector2Int coordinates) => GetTile(coordinates.x, coordinates.y);
    public List<MapTile> AllTiles => Tiles.Cast<MapTile>().ToList();
    public List<MapTile> OwnedTiles => AllTiles.Where(t => t.IsOwned).ToList();
}
