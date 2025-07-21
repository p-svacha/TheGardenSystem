using System.Collections.Generic;
using UnityEngine;

public class MapTile : INestedTooltipTarget
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

    public List<ObjectEffect> GetEffects()
    {
        if (HasObject) return Object.Effects;
        else return new List<ObjectEffect>();
    }

    public void Acquire() => IsOwned = true;
    public void PlaceObject(Object obj) => Object = obj;
    public void ClearObject() => Object = null;

    #region Getters

    public bool HasObject => Object != null;

    // Adjacent tiles
    public MapTile TileNorth => Map.GetTile(Coordinates + new Vector2Int(0, 1));
    public MapTile TileEast => Map.GetTile(Coordinates + new Vector2Int(1, 0));
    public MapTile TileSouth => Map.GetTile(Coordinates + new Vector2Int(0, -1));
    public MapTile TileWest => Map.GetTile(Coordinates + new Vector2Int(-1, 0));

    public MapTile TileNorthEast => Map.GetTile(Coordinates + new Vector2Int(1, 1));
    public MapTile TileSouthEast => Map.GetTile(Coordinates + new Vector2Int(1, -1));
    public MapTile TileSouthWest => Map.GetTile(Coordinates + new Vector2Int(-1, -1));
    public MapTile TileNorthWest => Map.GetTile(Coordinates + new Vector2Int(-1, 1));

    public List<MapTile> GetAdjacentTiles()
    {
        List<MapTile> adjTiles = new List<MapTile>();
        if (TileNorth != null) adjTiles.Add(TileNorth);
        if (TileEast != null) adjTiles.Add(TileEast);
        if (TileSouth != null) adjTiles.Add(TileSouth);
        if (TileWest != null) adjTiles.Add(TileWest);
        if (TileNorthEast != null) adjTiles.Add(TileNorthEast);
        if (TileSouthEast != null) adjTiles.Add(TileSouthEast);
        if (TileSouthWest != null) adjTiles.Add(TileSouthWest);
        if (TileNorthWest != null) adjTiles.Add(TileNorthWest);
        return adjTiles;
    }

    #endregion

    #region INestedTooltipTaget

    public string GetTooltipTitle() => "";
    public string GetToolTipBodyText(out List<INestedTooltipTarget> references)
    {
        references = new List<INestedTooltipTarget>();

        string bodyText = "";

        // General tile info
        bodyText += $"{Terrain.Def.GetNestedTooltipLink()} {Coordinates}";
        int ownedInfoSize = 14;
        string ownedText = IsOwned ? "Owned" : "Unowned";
        bodyText += $"\n<size={ownedInfoSize}>{ownedText}</size>";

        // Terrain
        bodyText += $"\n\n{Terrain.GetDescriptionForTileTooltip()}";

        // Object
        if (HasObject)
        {
            bodyText += $"\n\n{Object.GetNestedTooltipLink()}";
            references.Add(Object);
        }

        if (Game.Instance.GameState == GameState.ScatterManipulation)
        {
            Dictionary<ResourceDef, ResourceProduction> tileProduction = Game.Instance.GetTileProduction(this);

            if (tileProduction != null)
            {
                foreach (ResourceProduction prod in tileProduction.Values)
                {
                    int numProduced = prod.GetValue();
                    if (numProduced != 0)
                    {
                        bodyText += "\n\n" + prod.GetBreakdownString();
                    }
                }
            }
        }

        return bodyText;
    }

    public string NestedTooltipLinkId => $"MapTile_{Coordinates}";
    public string NestedTooltipLinkText => Terrain.LabelCap;
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
