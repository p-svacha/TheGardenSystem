using System.Collections.Generic;
using UnityEngine;

public class MapTile : INestedTooltipTarget
{
    public Map Map { get; private set; }
    public Vector2Int Coordinates { get; private set; }
    public Terrain Terrain { get; private set; }
    public Object Object { get; private set; }

    // Ownage properties
    public bool IsOwned { get; private set; }
    public int Claim { get; private set; }

    public MapTile(Map map, Vector2Int coordinates, TerrainDef terrainDef)
    {
        Map = map;
        Coordinates = coordinates;
        Terrain = new Terrain(this, terrainDef);
        IsOwned = false;
        Claim = 0;
    }

    public List<ObjectEffect> GetEffects()
    {
        List<ObjectEffect> effects = new List<ObjectEffect>();

        // Effects from terrain
        effects.AddRange(Terrain.Effects);

        // Effects from object
        if (HasObject) effects.AddRange(Object.Effects);

        return effects;
    }

    public void AdjustClaim(int amount)
    {
        Claim += amount;
        if (Claim >= Game.CLAIMS_NEEDED_TO_ACQUIRE_TILES) Game.Instance.AddTileToGarden(this);
        if (Claim < 0) Claim = 0;
    }

    public void Acquire()
    {
        IsOwned = true;
        Claim = Game.CLAIMS_NEEDED_TO_ACQUIRE_TILES;
    }
    public void PlaceObject(Object obj) => Object = obj;
    public void ClearObject() => Object = null;
    public void SetTerrain(TerrainDef def)
    {
        Terrain = new Terrain(this, def);
    }

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

    public List<MapTile> GetOrthogonalAdjacentTiles()
    {
        List<MapTile> adjTiles = new List<MapTile>();
        if (TileNorth != null) adjTiles.Add(TileNorth);
        if (TileEast != null) adjTiles.Add(TileEast);
        if (TileSouth != null) adjTiles.Add(TileSouth);
        if (TileWest != null) adjTiles.Add(TileWest);
        return adjTiles;
    }

    #endregion

    #region INestedTooltipTaget

    public string GetTooltipTitle() => "";
    public string GetToolTipBodyText(out List<INestedTooltipTarget> references)
    {
        references = new List<INestedTooltipTarget>();

        string bodyText = "";

        // Title (terrain + coordinates)
        bodyText += $"{Terrain.Def.GetTooltipLink()} {Coordinates}";

        // Ownage info
        int ownedInfoSize = 14;
        string ownedText = IsOwned ? "Owned" : $"Unowned ({Claim}/{Game.CLAIMS_NEEDED_TO_ACQUIRE_TILES} Claim)";
        bodyText += $"\n<size={ownedInfoSize}>{ownedText}</size>";

        // Terrain
        bodyText += $"\n\n{Terrain.GetDescriptionForTileTooltip()}";

        // Object
        if (HasObject)
        {
            bodyText += $"\n\n{Object.GetTooltipLink()}";
            references.Add(Object);
        }

        if (Game.Instance.GameState == GameState.ScatterManipulation)
        {
            Dictionary<ResourceDef, ResourceProduction> tileProduction = Game.Instance.GetTileProduction(this);

            if (tileProduction != null)
            {
                foreach (ResourceProduction prod in tileProduction.Values)
                {
                    int baseValue = prod.BaseValue;
                    int finalValue = prod.GetValue();
                    if (baseValue != 0 || finalValue != 0) // Show breakdown when either base or final value is not 0
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
