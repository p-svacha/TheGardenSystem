using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapTile : ITooltipTarget
{
    public Map Map { get; private set; }
    public Vector2Int Coordinates { get; private set; }
    public Terrain Terrain { get; private set; }
    public Object Object { get; private set; }

    // Ownage properties
    public bool IsOwned { get; private set; }
    public ResourceCollection AcquireCost { get; private set; }

    /// <summary>
    /// The modifiers applied to this tile, independent from object on it.
    /// </summary>
    public List<Modifier> Modifiers { get; private set; }

    public MapTile(Map map, Vector2Int coordinates, TerrainDef terrainDef)
    {
        Map = map;
        Coordinates = coordinates;
        Terrain = new Terrain(this, terrainDef);
        IsOwned = false;
        AcquireCost = GetOriginalAcquireCost();
        Modifiers = new List<Modifier>();
    }

    public List<ObjectEffect> GetEffects()
    {
        List<ObjectEffect> effects = new List<ObjectEffect>();

        // Effects from terrain
        effects.AddRange(Terrain.Effects);

        // Effects from tile modifiers
        foreach (Modifier modifier in Modifiers)
        {
            ObjectEffect modifierEffect = modifier.Effect.GetCopy();
            modifierEffect.EffectSource = modifier.Def;
            effects.Add(modifierEffect);
        }

        // Effects from object
        if (HasObject) effects.AddRange(Object.GetAllEffects());

        return effects;
    }

    public void Acquire()
    {
        IsOwned = true;
        AcquireCost = new ResourceCollection();
    }
    public void Unacquire()
    {
        IsOwned = false;
        AcquireCost = GetOriginalAcquireCost();
    }
    public void PlaceObject(Object obj) => Object = obj;
    public void ClearObject() => Object = null;
    public void SetTerrain(TerrainDef def)
    {
        Terrain = new Terrain(this, def);
    }

    #region Modifiers

    public void ApplyModifier(ModifierDef def, int duration = -1)
    {
        Debug.Log($"Applying modifier {def.DefName} to tile {Coordinates} with a duration of {duration}.");
        if (!def.IsStackable && HasModifier(def))
        {
            Modifier existingModifier = Modifiers.First(m => m.Def == def);
            if (existingModifier.IsInfinite) return;
            if (duration == -1) existingModifier.MakeInfinite();
            else if (duration > existingModifier.RemainingDuration) existingModifier.SetDuration(duration);
        }
        else Modifiers.Add(new Modifier(def, duration));
    }

    public void DecrementModifierDurations()
    {
        foreach (Modifier modifier in Modifiers) modifier.DecreaseDuration();
        Modifiers = Modifiers.Where(m => m.RemainingDuration == -1 || m.RemainingDuration > 0).ToList();
    }

    public bool HasModifier(ModifierDef def) => Modifiers.Any(m => m.Def == def);

    #endregion

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

    public List<MapTile> GetAdjacentTiles(int radius = 1)
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

        if (radius > 1)
        {
            int remRadius = radius;
            while(remRadius > 0)
            {
                List<MapTile> tilesToAdd = new List<MapTile>();
                foreach (MapTile tile in adjTiles)
                {
                    foreach (MapTile adjTile in tile.GetAdjacentTiles())
                    {
                        if (!adjTiles.Contains(adjTile) && adjTile != this) tilesToAdd.Add(adjTile);
                    }
                }
                adjTiles.AddRange(tilesToAdd);
                remRadius--;
            }
        }

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

    public ResourceCollection GetOriginalAcquireCost()
    {
        int ringIndex = Mathf.Max(Mathf.Abs(Coordinates.x), Mathf.Abs(Coordinates.y)) - 1;
        int goldCost = Game.COST_PER_TILE_RING * ringIndex;
        return new ResourceCollection(new Dictionary<ResourceDef, int>()
        {
            { ResourceDefOf.Gold, goldCost },
        });
    }

    #endregion

    #region ITooltipTaget

    public string GetTooltipTitle() => "";
    public string GetTooltipBodyText(List<ITooltipTarget> dynamicReferences)
    {
        string bodyText = "";

        // Title (terrain + coordinates)
        bodyText += $"{Terrain.Def.GetTooltipLink()} {Coordinates}";

        // Ownage info
        int ownedInfoSize = 14;
        string ownedText = IsOwned ? "Owned" : $"Unowned ({AcquireCost.GetAsSingleLinkedString()})";
        bodyText += $"\n<size={ownedInfoSize}>{ownedText}</size>";

        // Terrain
        bodyText += $"\n\n{Terrain.GetDescriptionForTileTooltip()}";

        // Modifiers
        if (Modifiers.Count > 0)
        {
            bodyText += "\n\nModifiers:";
            foreach (Modifier modifier in Modifiers)
            {
                string duration = modifier.IsInfinite ? "" : $" ({modifier.RemainingDuration} days remaining)";
                bodyText += $"\n{modifier.Def.GetTooltipLink()}: {modifier.Effect.GetDescription()}{duration}";
            }
        }

        // Object
        if (HasObject)
        {
            bodyText += $"\n\n{Object.GetTooltipLink()}";
            dynamicReferences.Add(Object);
        }

        // Production (only during scatter)
        if (Game.Instance.GameState == GameState.Afternoon)
        {
            Dictionary<ResourceDef, ResourceProduction> tileProduction = Game.Instance.GetTileProduction(this);

            if (tileProduction != null)
            {
                List<ResourceProduction> productionsToShow = tileProduction.Values.Where(p => p.BaseValue != 0 || p.GetValue() != 0).ToList();

                if (productionsToShow.Count > 0) bodyText += "\n";
                foreach (ResourceProduction prod in productionsToShow)
                {
                    int baseValue = prod.BaseValue;
                    int finalValue = prod.GetValue();
                    if (baseValue != 0 || finalValue != 0) // Show breakdown when either base or final value is not 0
                    {
                        bodyText += "\n" + prod.GetTooltipLink();
                        dynamicReferences.Add(prod);
                    }
                }
            }
        }

        return bodyText;
    }

    public string NestedTooltipLinkId => $"MapTile_{Coordinates}";
    public string NestedTooltipLinkText => Terrain.LabelCap;
    public Color NestedTooltipLinkColor => TooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
