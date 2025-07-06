using UnityEngine;

public class MapTile
{
    public Map Map { get; private set; }
    public Vector2Int Coordinates { get; private set; }
    public Terrain Terrain { get; private set; }
    public bool IsOwned { get; private set; }
    public Object Object { get; private set; }

    public MapTile(Map map, Vector2Int coordinates, TerrainDef terrainDef)
    {
        Map = map;
        Coordinates = coordinates;
        Terrain = new Terrain(this, terrainDef);
    }

    public void Acquire() => IsOwned = true;
    public void PlaceObject(Object obj) => Object = obj;
    public void ClearObject() => Object = null;

    #region Getters

    // Adjacent tiles
    public MapTile North => Map.GetTile(Coordinates + new Vector2Int(0, 1));
    public MapTile East => Map.GetTile(Coordinates + new Vector2Int(1, 0));
    public MapTile South => Map.GetTile(Coordinates + new Vector2Int(0, -1));
    public MapTile West => Map.GetTile(Coordinates + new Vector2Int(-1, 0));

    public MapTile NorthEast => Map.GetTile(Coordinates + new Vector2Int(1, 1));
    public MapTile SouthEast => Map.GetTile(Coordinates + new Vector2Int(1, -1));
    public MapTile SouthWest => Map.GetTile(Coordinates + new Vector2Int(-1, -1));
    public MapTile NorthWest => Map.GetTile(Coordinates + new Vector2Int(-1, 1));

    #endregion
}
