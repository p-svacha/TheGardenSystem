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
    public Dictionary<ResourceDef, ResourceProduction> CurrentFinalResourceProduction { get; private set; }
    public Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> CurrentPerTileResourceProduction { get; private set; }


    #region Initialization

    public Game()
    {
        Instance = this;
        Day = 1;
        Map = MapGenerator.GenerateMap(15);
        Resources = new Dictionary<ResourceDef, int>();
        CurrentFinalResourceProduction = new Dictionary<ResourceDef, ResourceProduction>();
        CurrentPerTileResourceProduction = new Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>>();
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
        AddObject(ObjectDefOf.CompostHeap);

        GameState = GameState.Uninitialized;
    }

    public void Initialize()
    {
        // Render
        DrawFullMap();
        CameraHandler.Instance.SetBounds(0, 0, Map.Width, Map.Height);
        CameraHandler.Instance.FocusPosition(new Vector2(Map.Width / 2f, Map.Height / 2f));

        // UI
        GameUI.Instance.DatePanel.Refresh();
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

        CurrentFinalResourceProduction = GetCurrentScatterProduction();

        DrawFullMap();

        GameState = GameState.ScatterManipulation;

        // UI
        GameUI.Instance.ResourcePanel.Refresh();
        NestedTooltipManager.Instance.ResetTooltips();
    }

    public void ConfirmScatter()
    {
        // Give resources
        Dictionary<ResourceDef, int> resources = new Dictionary<ResourceDef, int>();
        CurrentFinalResourceProduction = GetCurrentScatterProduction();
        foreach (var kvp in CurrentFinalResourceProduction)
        {
            ResourceDef resource = kvp.Key;
            ResourceProduction production = kvp.Value;

            resources.Increment(resource, production.GetValue());
        }

        AddResources(resources);

        GameState = GameState.ConfirmedScatter;

        // UI
        GameUI.Instance.ResourcePanel.Refresh();
        NestedTooltipManager.Instance.ResetTooltips();
    }

    /// <summary>
    /// Calculates and returns the final resource productions of the day.
    /// </summary>
    private Dictionary<ResourceDef, ResourceProduction> GetCurrentScatterProduction()
    {
        // Create base resource productions for each tile in the garden
        CurrentPerTileResourceProduction.Clear();
        foreach (MapTile tile in Map.OwnedTiles)
        {
            CurrentPerTileResourceProduction.Add(tile, new Dictionary<ResourceDef, ResourceProduction>());

            Dictionary<ResourceDef, int> baseProduction = tile.HasObject ? tile.Object.GetBaseResourceProduction() : new();
            string label = tile.HasObject ? tile.Object.LabelCap : "Empty Tile";

            foreach (ResourceDef resource in DefDatabase<ResourceDef>.AllDefs)
            {
                int baseValue = 0;
                baseProduction.TryGetValue(resource, out baseValue);
                string id = $"{Day}_{tile.Coordinates}_{resource.DefName}";
                CurrentPerTileResourceProduction[tile].Add(resource, new ResourceProduction(id, label, resource, baseValue));
            }
        }

        // Apply modifiers from each object to all other affected objects
        foreach (MapTile tile in Map.OwnedTiles)
        {
            foreach(ObjectEffect effect in tile.GetEffects())
            {
                effect.ApplyEffect(tile, CurrentPerTileResourceProduction);
            }
        }

        // Create an final resource productions for each resource
        Dictionary<ResourceDef, ResourceProduction> finalProduction = new Dictionary<ResourceDef, ResourceProduction>();
        foreach (ResourceDef resource in DefDatabase<ResourceDef>.AllDefs)
        {
            string id = $"{Day}_final_{resource.DefName}";
            finalProduction.Add(resource, new ResourceProduction(id, "Daily Production", resource, 0));
        }

        // Add resources from every tile to the final production
        foreach (KeyValuePair<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProduction in CurrentPerTileResourceProduction)
        {
            MapTile tile = tileProduction.Key;
            Object obj = tile.Object;
            Dictionary<ResourceDef, ResourceProduction> objectProduction = tileProduction.Value;

            foreach (KeyValuePair<ResourceDef, ResourceProduction> objectResourceProduction in objectProduction)
            {
                ResourceDef resource = objectResourceProduction.Key;
                ResourceProduction production = objectResourceProduction.Value;
                int numResourcesProduced = production.GetValue();

                if (numResourcesProduced != 0)
                {
                    finalProduction[resource].AddModifier(new ProductionModifier(production, ProductionModifierType.Additive, production.GetValue()));
                }
            }
        }

        return finalProduction;
    }

    public void EndDay()
    {
        Day++;

        Map.ClearAllObjects();
        DrawFullMap();

        CurrentFinalResourceProduction.Clear();

        GameState = GameState.BeforeScatter;

        // UI
        GameUI.Instance.DatePanel.Refresh();
        GameUI.Instance.ResourcePanel.Refresh();
        NestedTooltipManager.Instance.ResetTooltips();
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

    #region Getters

    public string GetWeekdayName()
    {
        string[] weekdayNames = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        int index = (Day - 1) % 7;
        return weekdayNames[index];
    }

    public int GetWeekNumber()
    {
        return ((Day - 1) / 7) + 1;
    }

    public Dictionary<ResourceDef, ResourceProduction> GetTileProduction(MapTile tile)
    {
        if (CurrentPerTileResourceProduction.TryGetValue(tile, out var prod)) return prod;
        return null;
    }

    #endregion
}
