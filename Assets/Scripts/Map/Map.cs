using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Map
{
    private Dictionary<Vector2Int, MapTile> Tiles;

    public void Initialize(Dictionary<Vector2Int, MapTile> tiles)
    {
        Tiles = tiles;
    }

    public void ClearAllObjects()
    {
        foreach (MapTile tile in AllTiles) tile.ClearObject();
    }

    public MapTile GetTile(int x, int y) => GetTile(new Vector2Int(x, y));

    public MapTile GetTile(Vector2Int coordinates)
    {
        if (Tiles.TryGetValue(coordinates, out MapTile tile)) return tile;
        return null;
    }

    public void SetTerrain(Vector2Int coordinates, TerrainDef def)
    {
        Tiles[coordinates].SetTerrain(def);
    }

    public int MinX => Tiles.Keys.Min(x => x.x);
    public int MaxX => Tiles.Keys.Max(x => x.x);
    public int MinY => Tiles.Keys.Min(x => x.y);
    public int MaxY => Tiles.Keys.Max(x => x.y);
    public int Width => MaxX - MinX + 1;
    public int Height => MaxY - MinY + 1;
    public List<MapTile> AllTiles => Tiles.Values.ToList();
    public List<MapTile> OwnedTiles => AllTiles.Where(t => t.IsOwned).ToList();
}
