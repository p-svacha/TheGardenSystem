using UnityEngine;
using System.Collections.Generic;

public class Game
{
    public static Game Instance;
    public const int STARTING_AREA_SIZE = 3;

    public GameState GameState { get; private set; }
    public int Day { get; private set; }
    public Map Map { get; private set; }
    public Dictionary<ResourceDef, int> Resources { get; private set; }
    public List<Object> Objects { get; private set; }


    #region Initialization

    public Game()
    {
        Instance = this;
        Day = 1;
        Map = MapGenerator.GenerateMap(15);
        Resources = new Dictionary<ResourceDef, int>();
        foreach (ResourceDef def in DefDatabase<ResourceDef>.AllDefs) Resources.Add(def, 0);

        // Starting garden area
        int gardenStartX = (int)((Map.Width / 2f) - (STARTING_AREA_SIZE / 2f)); 
        int gardenStartY = (int)((Map.Height / 2f) - (STARTING_AREA_SIZE / 2f));
        for(int x = gardenStartX; x < gardenStartX + STARTING_AREA_SIZE; x++)
        {
            for(int y = gardenStartY; y < gardenStartY + STARTING_AREA_SIZE; y++)
            {
                AddTileToGarden(Map.GetTile(x, y), redraw: false);
            }
        }

        // Starting objects
        Objects = new List<Object>();
        AddObject(ObjectDefOf.Carrot);

        GameState = GameState.Uninitialized;
    }

    public void Initialize()
    {
        // Render
        DrawFullMap();
        CameraHandler.Instance.SetBounds(0, 0, Map.Width, Map.Height);
        CameraHandler.Instance.FocusPosition(new Vector2(Map.Width / 2f, Map.Height / 2f));

        // UI
        GameUI.Instance.ResourcePanel.Refresh();

        // State
        GameState = GameState.BeforeScatter;
    }

    #endregion

    #region Game Loop

    public void HandleInputs()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (GameState == GameState.BeforeScatter) StartDay();
            else if (GameState == GameState.ScatterManipulation) ConfirmScatter();
            else if (GameState == GameState.ConfirmedScatter) EndDay();
        }
    }

    /// <summary>
    /// Scatters the objects around the garden.
    /// </summary>
    public void StartDay()
    {
        List<Object> remainingObjects = new List<Object>(Objects);
        List<MapTile> shuffledGardenTiles = Map.OwnedTiles.GetShuffledList();

        foreach (MapTile tile in shuffledGardenTiles)
        {
            if (remainingObjects.Count == 0) break;

            Object pickedObject = remainingObjects.RandomElement();
            tile.PlaceObject(pickedObject);
            remainingObjects.Remove(pickedObject);
        }

        DrawFullMap();

        GameState = GameState.ScatterManipulation;
    }

    public void ConfirmScatter()
    {
        // Give resources
        Dictionary<ResourceDef, int> resources = new Dictionary<ResourceDef, int>();
        foreach(MapTile tile in Map.OwnedTiles)
        {
            if (tile.Object != null)
            {
                resources.IncrementMultiple(tile.Object.GetBaseResources());
            }
        }

        AddResources(resources);

        GameState = GameState.ConfirmedScatter;
    }

    public void EndDay()
    {
        Day++;
        Map.ClearAllObjects();
        GameState = GameState.BeforeScatter;
    }

    #endregion

    #region Actions

    public void AddObject(ObjectDef def)
    {
        Object newObj = new Object(def);
        Objects.Add(newObj);
    }

    public void AddTileToGarden(MapTile tile, bool redraw = true)
    {
        tile.Acquire();
        if(redraw) DrawFullMap();
    }

    public void AddResources(Dictionary<ResourceDef, int> res)
    {
        Resources.IncrementMultiple(res);

        GameUI.Instance.ResourcePanel.Refresh();
    }

    #endregion

    #region Render

    public void DrawFullMap()
    {
        MapRenderer.Instance.DrawFullMap(Map);
    }

    #endregion
}
