using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Game
{
    public static Game Instance;
    public const int STARTING_AREA_SIZE = 3;
    public const int DAYS_PER_WEEK = 7;

    public GameState GameState { get; private set; }
    public int Day { get; private set; }
    public Map Map { get; private set; }
    public ResourceCollection Resources { get; private set; }
    public List<Object> Objects { get; private set; }
    public Dictionary<ResourceDef, ResourceProduction> CurrentFinalResourceProduction { get; private set; }
    public Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> CurrentPerTileResourceProduction { get; private set; }
    public List<Order> ActiveOrders { get; private set; }


    #region Initialization

    public Game()
    {
        Instance = this;
        Day = 1;
        Map = MapGenerator.GenerateMap(15);
        Resources = new ResourceCollection();
        CurrentFinalResourceProduction = new Dictionary<ResourceDef, ResourceProduction>();
        CurrentPerTileResourceProduction = new Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>>();
        foreach (ResourceDef def in DefDatabase<ResourceDef>.AllDefs) Resources.Resources.Add(def, 0);

        // Starting garden area
        int gardenStartX = (int)((Map.Width / 2f) - (STARTING_AREA_SIZE / 2f));
        int gardenStartY = (int)((Map.Height / 2f) - (STARTING_AREA_SIZE / 2f));
        for (int x = gardenStartX; x < gardenStartX + STARTING_AREA_SIZE; x++)
        {
            for (int y = gardenStartY; y < gardenStartY + STARTING_AREA_SIZE; y++)
            {
                AddTileToGarden(Map.GetTile(x, y), redraw: false);
            }
        }

        // Starting objects
        Objects = new List<Object>();
        AddObjectToInventory(ObjectDefOf.Carrot);
        AddObjectToInventory(ObjectDefOf.CompostHeap);

        // Starting orders
        ActiveOrders = new List<Order>();
        ResourceCollection firstWeekOrderResources = new ResourceCollection(new Dictionary<ResourceDef, int>()
        {
            { ResourceDefOf.Food, 20 },
        });
        ActiveOrders.Add(new Order(week: 1, firstWeekOrderResources));

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
        GameUI.Instance.OrderPanel.Refresh();

        // State
        GameState = GameState.BeforeScatter;
    }

    #endregion

    #region Game Loop

    public void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameState == GameState.BeforeScatter) StartDay();
            else if (GameState == GameState.ScatterManipulation) ConfirmScatter();
            else if (GameState == GameState.ConfirmedScatter) StartPostScatter();
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

    private void ConfirmScatter()
    {
        // Give resources
        ResourceCollection resources = new ResourceCollection();
        CurrentFinalResourceProduction = GetCurrentScatterProduction();
        foreach (var kvp in CurrentFinalResourceProduction)
        {
            ResourceDef resource = kvp.Key;
            ResourceProduction production = kvp.Value;

            resources.AddResource(resource, production.GetValue());
        }

        AddResources(resources);

        GameState = GameState.ConfirmedScatter;

        // UI
        GameUI.Instance.ResourcePanel.Refresh();
        NestedTooltipManager.Instance.ResetTooltips();
    }

    private void StartPostScatter()
    {
        if (IsLastDayOfWeek)
        {
            DeliverOrders();
            CreateNextWeeksOrders();

            // UI
            GameUI.Instance.ResourcePanel.Refresh();
            GameUI.Instance.OrderPanel.Refresh();
        }
        StartObjectDraft();
    }

    private void DeliverOrders()
    {
        foreach (Order order in DueOrders)
        {
            if (!Resources.HasResources(order.OrderedResources))
            {
                Debug.Log("YOU LOSE YOU LOSE YOU LOSE");
            }
            Resources.RemoveResources(order.OrderedResources);
        }
    }
    /// <summary>
    /// Removes the completed orders and creates next weeks orders.
    /// </summary>
    private void CreateNextWeeksOrders()
    {
        foreach (Order order in DueOrders)
        {
            ResourceCollection nextWeeksOrder = new ResourceCollection(order.OrderedResources);

            foreach (ResourceDef res in nextWeeksOrder.GetResourceList())
            {
                float multiplier = Random.Range(1.1f, 3f);
                int targetValue = (int)(nextWeeksOrder.Resources[res] * multiplier);
                int numToAdd = targetValue - nextWeeksOrder.Resources[res];
                nextWeeksOrder.AddResource(res, numToAdd);
            }
            ActiveOrders.Add(new Order(GetWeekNumber() + 1, nextWeeksOrder));
        }
        ActiveOrders = ActiveOrders.Where(o => o.DueDay != Day).ToList();
    }

    private void StartObjectDraft()
    {
        GameState = GameState.ObjectDraft;

        // Create candidate probability table
        Dictionary<ObjectDef, float> candidates = new Dictionary<ObjectDef, float>();
        foreach (ObjectDef def in DefDatabase<ObjectDef>.AllDefs) candidates.Add(def, 1f);

        // Choose draft options out of candidates
        List<ObjectDef> draftOptions = candidates.GetWeightedRandomElements(amount: 3, allowRepeating: false);
        List<IDraftable> draftableOptions = draftOptions.Select(o => (IDraftable)o).ToList();

        // Show draft window
        string draftWindowTitle = $"Day {Day} Complete";
        if (IsLastDayOfWeek) draftWindowTitle = $"Week {GetWeekNumber()} Complete\nAll orders have been delivered.";
        string draftWindowSubtitle = "Choose an object to add to your inventory";
        UI_DraftWindow.Instance.Show(draftWindowTitle, draftWindowSubtitle, draftableOptions, isDraft: true, OnObjectDrafted);
    }

    private void OnObjectDrafted(List<IDraftable> selectedOptions)
    {
        // Add drafted objects to inventory
        foreach(IDraftable draftedObject in selectedOptions)
        {
            ObjectDef def = (ObjectDef)draftedObject;
            AddObjectToInventory(def);
        }

        // End day
        EndDay();
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

            ResourceCollection baseProduction = tile.HasObject ? tile.Object.GetNativeResourceProduction() : new();
            string label = tile.HasObject ? tile.Object.LabelCap : "Empty Tile";

            foreach (ResourceDef resource in DefDatabase<ResourceDef>.AllDefs)
            {
                int baseValue;
                if(!baseProduction.Resources.TryGetValue(resource, out baseValue)) baseValue = 0;
                string id = $"{Day}_{tile.Coordinates}_{resource.DefName}";
                CurrentPerTileResourceProduction[tile].Add(resource, new ResourceProduction(id, label, resource, baseValue));
            }
        }

        // Apply modifiers from each object to all other affected objects
        foreach (MapTile tile in Map.OwnedTiles)
        {
            foreach (ObjectEffect effect in tile.GetEffects())
            {
                if (!effect.Validate(out string invalidReason)) throw new System.Exception($"Cannot apply invalid effect of {tile}. ValidationFailReason: {invalidReason}");
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
        GameUI.Instance.OrderPanel.Refresh();
        NestedTooltipManager.Instance.ResetTooltips();
    }

    #endregion

    #region Actions

    public void AddObjectToInventory(ObjectDef def)
    {
        Object newObj = new Object(def);
        Objects.Add(newObj);
    }

    public void AddTileToGarden(MapTile tile, bool redraw = true)
    {
        tile.Acquire();
        if(redraw) DrawFullMap();
    }

    public void AddResources(ResourceCollection res)
    {
        Resources.AddResources(res);

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
        int index = (Day - 1) % DAYS_PER_WEEK;
        return weekdayNames[index];
    }

    public int GetWeekNumber()
    {
        return ((Day - 1) / DAYS_PER_WEEK) + 1;
    }

    public bool IsLastDayOfWeek => (Day - 1) % DAYS_PER_WEEK == DAYS_PER_WEEK - 1;

    public Dictionary<ResourceDef, ResourceProduction> GetTileProduction(MapTile tile)
    {
        if (CurrentPerTileResourceProduction.TryGetValue(tile, out var prod)) return prod;
        return null;
    }

    public List<Order> DueOrders => ActiveOrders.Where(o => o.DueDay == Day).ToList();

    #endregion
}
