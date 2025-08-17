using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;
using System.Linq;

public class MapRenderer : MonoBehaviour
{
    public static MapRenderer Instance;

    // Tilemaps
    public Grid TilemapGrid;
    private Tilemap TerrainTilemap;
    private List<Tilemap> BlendOverlayTilemaps;
    private Dictionary<Direction, Tilemap> TerrainBlendTilemaps;

    private Tilemap[] TileModifierTilemaps;
    private Tilemap GridOverlayTilemap;
    private Dictionary<Direction, Tilemap> FenceTilemaps;
    public Tilemap ObjectTilemap;
    private Tilemap[] ObjectOverlayTilemaps;

    // Tile caches
    private Tile GridOverlayTile;
    private Tile FertilityOverlayTile;
    private Tile NegativeFertilityOverlayTile;
    private Dictionary<int, Tile> ClaimTiles;
    private Dictionary<TerrainDef, Tile> TerrainTileCache;
    private Dictionary<Direction, Tile> FenceTileCache;
    private Dictionary<ObjectDef, Tile> ObjectTileCache;
    private Dictionary<ModifierDef, Tile> ModifierTileCache;

    public Tile ShedOpenTile;

    #region Initialize

    private void Awake()
    {
        Instance = this;

        TilemapGrid = GetComponent<Grid>();
        TerrainTilemap = GameObject.Find("TerrainTilemap").GetComponent<Tilemap>();
        BlendOverlayTilemaps = new List<Tilemap>();
        for (int i = 1; i <= 9; i++) BlendOverlayTilemaps.Add(GameObject.Find("TerrainOverlayTilemap_Density" + i).GetComponent<Tilemap>());

        TerrainBlendTilemaps = new Dictionary<Direction, Tilemap>();
        TerrainBlendTilemaps.Add(Direction.N, GameObject.Find("TerrainBlend_N").GetComponent<Tilemap>());
        TerrainBlendTilemaps.Add(Direction.E, GameObject.Find("TerrainBlend_E").GetComponent<Tilemap>());
        TerrainBlendTilemaps.Add(Direction.S, GameObject.Find("TerrainBlend_S").GetComponent<Tilemap>());
        TerrainBlendTilemaps.Add(Direction.W, GameObject.Find("TerrainBlend_W").GetComponent<Tilemap>());

        TileModifierTilemaps = new Tilemap[9];
        for (int i = 1; i <= 9; i++) TileModifierTilemaps[i - 1] = GameObject.Find("TileModifierTilemap" + i).GetComponent<Tilemap>();

        GridOverlayTilemap = GameObject.Find("GridOverlayTilemap").GetComponent<Tilemap>();

        FenceTilemaps = new Dictionary<Direction, Tilemap>();
        FenceTilemaps.Add(Direction.N, GameObject.Find("FenceTilemap_N").GetComponent<Tilemap>());
        FenceTilemaps.Add(Direction.E, GameObject.Find("FenceTilemap_E").GetComponent<Tilemap>());
        FenceTilemaps.Add(Direction.S, GameObject.Find("FenceTilemap_S").GetComponent<Tilemap>());
        FenceTilemaps.Add(Direction.W, GameObject.Find("FenceTilemap_W").GetComponent<Tilemap>());

        ObjectTilemap = GameObject.Find("ObjectTilemap").GetComponent<Tilemap>();

        ObjectOverlayTilemaps = new Tilemap[9];
        for (int i = 1; i <= 9; i++)
        {
            ObjectOverlayTilemaps[i - 1] = GameObject.Find("ObjectModifierTilemap" + i).GetComponent<Tilemap>();
        }
    }

    private void Start()
    {
        InitializeTerrainTiles();
        InitializeOverlayTiles();
        InitializeFenceTiles();
        InitializeObjectTiles();
        InitializeModifierTiles();
    }

    /// <summary>
    /// Create one reusable Tile asset for each TerrainDef, keyed by def.
    /// </summary>
    private void InitializeTerrainTiles()
    {
        TerrainTileCache = new Dictionary<TerrainDef, Tile>();

        foreach (var def in DefDatabase<TerrainDef>.AllDefs)
        {
            Tile tile = CreateTileFromSprite(def.Sprite);
            TerrainTileCache[def] = tile;
        }
    }

    /// <summary>
    /// Creates reusable Tiles used for overlays.
    /// </summary>
    private void InitializeOverlayTiles()
    {
        // Grid
        GridOverlayTile = CreateTileFromSprite(ResourceManager.LoadSprite("Sprites/Overlays/TileGridOverlay"));
        FertilityOverlayTile = CreateTileFromSprite(ResourceManager.LoadSprite("Sprites/Terrain/Overlays/Fertility"));
        NegativeFertilityOverlayTile = CreateTileFromSprite(ResourceManager.LoadSprite("Sprites/Terrain/Overlays/FertilityNegative"));
    }

    private void InitializeFenceTiles()
    {
        FenceTileCache = new Dictionary<Direction, Tile>();

        FenceTileCache.Add(Direction.N, CreateTileFromSprite((ResourceManager.LoadSprite("Sprites/Overlays/Fence_N"))));
        FenceTileCache.Add(Direction.E, CreateTileFromSprite((ResourceManager.LoadSprite("Sprites/Overlays/Fence_E"))));
        FenceTileCache.Add(Direction.S, CreateTileFromSprite((ResourceManager.LoadSprite("Sprites/Overlays/Fence_S"))));
        FenceTileCache.Add(Direction.W, CreateTileFromSprite((ResourceManager.LoadSprite("Sprites/Overlays/Fence_W"))));
    }

    private void InitializeObjectTiles()
    {
        ObjectTileCache = new Dictionary<ObjectDef, Tile>();

        foreach (var def in DefDatabase<ObjectDef>.AllDefs)
        {
            Tile tile = CreateTileFromSprite(def.Sprite);
            ObjectTileCache[def] = tile;
        }

        // Special tiles
        ShedOpenTile = CreateTileFromSprite(ResourceManager.LoadSprite("Sprites/Objects/Shed_Open"));
    }

    private void InitializeModifierTiles()
    {
        ModifierTileCache = new Dictionary<ModifierDef, Tile>();

        foreach (var def in DefDatabase<ModifierDef>.AllDefs)
        {
            Tile tile = CreateTileFromSprite(def.Sprite);
            ModifierTileCache[def] = tile;
        }
    }

    private Tile CreateTileFromSprite(Sprite sprite)
    {
        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.None;
        return tile;
    }

    #endregion

    #region Draw

    public void DrawFullMap(Map map)
    {
        TerrainTilemap.ClearAllTiles();
        foreach(Tilemap tilemap in BlendOverlayTilemaps) tilemap.ClearAllTiles();
        GridOverlayTilemap.ClearAllTiles();
        foreach (Tilemap tilemap in TerrainBlendTilemaps.Values) tilemap.ClearAllTiles();
        foreach (Tilemap tilemap in FenceTilemaps.Values) tilemap.ClearAllTiles();
        foreach (Tilemap tilemap in ObjectOverlayTilemaps) tilemap.ClearAllTiles();
        foreach (Tilemap tilemap in TileModifierTilemaps) tilemap.ClearAllTiles();

        foreach (MapTile mapTile in map.AllTiles)
        {
            Vector3Int cell = new Vector3Int(mapTile.Coordinates.x, mapTile.Coordinates.y, 0);

            // Terrain
            if (!TerrainTileCache.TryGetValue(mapTile.Terrain.Def, out var tile)) continue;
            TerrainTilemap.SetTile(cell, tile);

            // Terrain Blending
            if (mapTile.TileNorth != null && mapTile.TileNorth.Terrain.Def != mapTile.Terrain.Def) TerrainBlendTilemaps[Direction.N].SetTile(cell, TerrainTileCache[mapTile.TileNorth.Terrain.Def]);
            if (mapTile.TileEast != null && mapTile.TileEast.Terrain.Def != mapTile.Terrain.Def) TerrainBlendTilemaps[Direction.E].SetTile(cell, TerrainTileCache[mapTile.TileEast.Terrain.Def]);
            if (mapTile.TileSouth != null && mapTile.TileSouth.Terrain.Def != mapTile.Terrain.Def) TerrainBlendTilemaps[Direction.S].SetTile(cell, TerrainTileCache[mapTile.TileSouth.Terrain.Def]);
            if (mapTile.TileWest != null && mapTile.TileWest.Terrain.Def != mapTile.Terrain.Def) TerrainBlendTilemaps[Direction.W].SetTile(cell, TerrainTileCache[mapTile.TileWest.Terrain.Def]);

            // Terrain Overlay
            if (mapTile.Terrain.IsAffectedByFertility)
            {
                if (mapTile.Terrain.Fertility > 0 && mapTile.Terrain.HasNextFertilityLevel)
                {
                    if (BlendOverlayTilemaps.Count <= mapTile.Terrain.Fertility - 1) throw new System.Exception($"No density tilemap found for density {mapTile.Terrain.Fertility}");
                    BlendOverlayTilemaps[mapTile.Terrain.Fertility - 1].SetTile(cell, FertilityOverlayTile);
                }
                if (mapTile.Terrain.Fertility < 0 && mapTile.Terrain.HasPrevFertilityLevel)
                {
                    BlendOverlayTilemaps[Math.Abs(mapTile.Terrain.Fertility) - 1].SetTile(cell, NegativeFertilityOverlayTile);
                }
            }

            // Tile Modifiers
            int index = 0;
            HashSet<ModifierDef> tileModiferDefSet = mapTile.Modifiers.Select(m => m.Def).ToHashSet();
            foreach (ModifierDef def in tileModiferDefSet)
            {
                TileModifierTilemaps[index].SetTile(cell, ModifierTileCache[def]);
                index++;
            }

            // Grid Overlay
            if (Game.Instance.IsShowingGridOverlay) GridOverlayTilemap.SetTile(cell, GridOverlayTile);

            // Fences
            if (mapTile.IsOwned) DrawFencesAround(mapTile);

            // Object
            if (mapTile.Object == null) ObjectTilemap.SetTile(cell, null);
            else
            {
                ObjectTilemap.SetTile(cell, ObjectTileCache[mapTile.Object.Def]);

                // Special cases

                // Open shed
                bool isShed = mapTile.Object.Def == ObjectDefOf.Shed;
                if (isShed)
                {
                    bool isDoorOpen = false;
                    if (UI_ShedWindow.Instance.gameObject.activeSelf && UI_ShedWindow.Instance.DisplayedSector == mapTile.Sector) isDoorOpen = true;
                    if (Game.Instance.GameState == GameState.Noon) isDoorOpen = true;
                    if (isDoorOpen) ObjectTilemap.SetTile(cell, ShedOpenTile);
                }

                // Object modifiers
                index = 0;
                HashSet<ModifierDef> modiferDefSet = mapTile.Object.Modifiers.Select(m => m.Def).ToHashSet();
                foreach (ModifierDef def in modiferDefSet)
                {
                    ObjectOverlayTilemaps[index].SetTile(cell, ModifierTileCache[def]);
                    index++;
                }
            }
        }

        // Force a redraw
        TerrainTilemap.RefreshAllTiles();
        GridOverlayTilemap.RefreshAllTiles();
        foreach (Tilemap tilemap in FenceTilemaps.Values) tilemap.RefreshAllTiles();
        ObjectTilemap.RefreshAllTiles();
    }

    private void DrawFencesAround(MapTile tile)
    {
        if (tile.TileNorth == null || !tile.TileNorth.IsOwned) DrawFence(tile, Direction.N);
        if (tile.TileEast == null || !tile.TileEast.IsOwned) DrawFence(tile, Direction.E);
        if (tile.TileSouth == null || !tile.TileSouth.IsOwned) DrawFence(tile, Direction.S);
        if (tile.TileWest == null || !tile.TileWest.IsOwned) DrawFence(tile, Direction.W);
    }

    private void DrawFence(MapTile tile, Direction dir)
    {
        // Look up our prebuilt tile with baked transform
        if (!FenceTileCache.TryGetValue(dir, out var fenceTile)) return;

        var pos = tile.Coordinates; // Vector2Int
        var cell = new Vector3Int(pos.x, pos.y, 0);
        FenceTilemaps[dir].SetTile(cell, fenceTile);
    }

    #endregion
}
