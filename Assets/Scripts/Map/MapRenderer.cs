using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class MapRenderer : MonoBehaviour
{
    public static MapRenderer Instance;

    // Tilemaps
    private Grid TilemapGrid;
    private Tilemap TerrainTilemap;
    private Tilemap TerrainOverlayTilemap;
    private Tilemap GridOverlayTilemap;
    private Dictionary<Direction, Tilemap> FenceTilemaps;
    private Tilemap ObjectTilemap;

    // Tile caches
    private Tile GridOverlayTile;
    private Tile FertilityOverlayTile;
    private Tile NegativeFertilityOverlayTile;
    private Dictionary<TerrainDef, Tile> TerrainTileCache;
    private Dictionary<Direction, Tile> FenceTileCache;
    private Dictionary<ObjectDef, Tile> ObjectTileCache;

    private void Awake()
    {
        Instance = this;

        TilemapGrid = GetComponent<Grid>();
        TerrainTilemap = GameObject.Find("TerrainTilemap").GetComponent<Tilemap>();
        TerrainOverlayTilemap = GameObject.Find("TerrainOverlayTilemap").GetComponent<Tilemap>();
        GridOverlayTilemap = GameObject.Find("GridOverlayTilemap").GetComponent<Tilemap>();

        FenceTilemaps = new Dictionary<Direction, Tilemap>();
        FenceTilemaps.Add(Direction.N, GameObject.Find("FenceTilemap_N").GetComponent<Tilemap>());
        FenceTilemaps.Add(Direction.E, GameObject.Find("FenceTilemap_E").GetComponent<Tilemap>());
        FenceTilemaps.Add(Direction.S, GameObject.Find("FenceTilemap_S").GetComponent<Tilemap>());
        FenceTilemaps.Add(Direction.W, GameObject.Find("FenceTilemap_W").GetComponent<Tilemap>());

        ObjectTilemap = GameObject.Find("ObjectTilemap").GetComponent<Tilemap>();
    }

    private void Start()
    {
        InitializeTerrainTiles();
        InitializeOverlayTiles();
        InitializeFenceTiles();
        InitializeObjectTiles();
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
        GridOverlayTile = CreateTileFromSprite(ResourceManager.LoadSprite("Sprites/TileGridOverlay"));
        FertilityOverlayTile = CreateTileFromSprite(ResourceManager.LoadSprite("Sprites/Terrain/Overlays/Fertility"));
        NegativeFertilityOverlayTile = CreateTileFromSprite(ResourceManager.LoadSprite("Sprites/Terrain/Overlays/FertilityNegative"));
    }

    private void InitializeFenceTiles()
    {
        FenceTileCache = new Dictionary<Direction, Tile>();

        // Load the base vertical fence sprite once
        Sprite baseFence = ResourceManager.LoadSprite("Sprites/Fence");

        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            Tile tile = CreateTileFromSprite(baseFence);

            // Compute a transform that both rotates and offsets the tile inside its cell
            Vector3 offset = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            float centerOffset = 0.45f;

            switch (dir)
            {
                case Direction.N:
                    offset = new Vector3(0f, centerOffset, 0f);
                    rot = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.E:
                    offset = new Vector3(centerOffset, 0f, 0f);
                    rot = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.S:
                    offset = new Vector3(0f, -centerOffset, 0f);
                    rot = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.W:
                    offset = new Vector3(-centerOffset, 0f, 0f);
                    rot = Quaternion.Euler(0, 0, 0);
                    break;
            }

            // Bake into the Tile’s transform
            tile.transform = Matrix4x4.TRS(offset, rot, Vector3.one);
            FenceTileCache[dir] = tile;
        }
    }

    private void InitializeObjectTiles()
    {
        ObjectTileCache = new Dictionary<ObjectDef, Tile>();

        foreach (var def in DefDatabase<ObjectDef>.AllDefs)
        {
            Tile tile = CreateTileFromSprite(def.Sprite);
            ObjectTileCache[def] = tile;
        }
    }

    private Tile CreateTileFromSprite(Sprite sprite)
    {
        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.None;
        return tile;
    }

    public void DrawFullMap(Map map)
    {
        TerrainTilemap.ClearAllTiles();
        TerrainOverlayTilemap.ClearAllTiles();
        GridOverlayTilemap.ClearAllTiles();

        int width = map.Width;
        int height = map.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                // Terrain
                MapTile mapTile = map.GetTile(x, y);
                if (!TerrainTileCache.TryGetValue(mapTile.Terrain.Def, out var tile)) continue;
                TerrainTilemap.SetTile(cell, tile);

                // Terrain Overlay
                if (mapTile.Terrain.IsAffectedByFertility)
                {
                    if (mapTile.Terrain.Fertility > 0)
                    {
                        float transparency = mapTile.Terrain.Fertility / 10f;
                        transparency *= 0.5f;
                        TerrainOverlayTilemap.SetTile(cell, FertilityOverlayTile);
                        TerrainOverlayTilemap.SetTileFlags(cell, TileFlags.None);
                        TerrainOverlayTilemap.SetColor(cell, new Color(1f, 1f, 1f, transparency));
                    }
                    if (mapTile.Terrain.Fertility < 0)
                    {
                        float transparency = -mapTile.Terrain.Fertility / 10f;
                        transparency *= 0.5f;
                        TerrainOverlayTilemap.SetTile(cell, NegativeFertilityOverlayTile);
                        TerrainOverlayTilemap.SetTileFlags(cell, TileFlags.None);
                        TerrainOverlayTilemap.SetColor(cell, new Color(1f, 1f, 1f, transparency));
                    }
                }

                // Grid Overlay
                if (Game.Instance.IsShowingGridOverlay) GridOverlayTilemap.SetTile(cell, GridOverlayTile);

                // Fences
                if (mapTile.IsOwned) DrawFencesAround(mapTile);

                // Object
                if (mapTile.Object == null) ObjectTilemap.SetTile(cell, null);
                else ObjectTilemap.SetTile(cell, ObjectTileCache[mapTile.Object.Def]);
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
        if (!tile.TileNorth.IsOwned) DrawFence(tile, Direction.N);
        if (!tile.TileEast.IsOwned) DrawFence(tile, Direction.E);
        if (!tile.TileSouth.IsOwned) DrawFence(tile, Direction.S);
        if (!tile.TileWest.IsOwned) DrawFence(tile, Direction.W);
    }

    private void DrawFence(MapTile tile, Direction dir)
    {
        // Look up our prebuilt tile with baked transform
        if (!FenceTileCache.TryGetValue(dir, out var fenceTile)) return;

        var pos = tile.Coordinates; // Vector2Int
        var cell = new Vector3Int(pos.x, pos.y, 0);
        FenceTilemaps[dir].SetTile(cell, fenceTile);
    }
}
