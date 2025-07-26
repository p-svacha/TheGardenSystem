using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Game
{
    public static Game Instance;
    public const int STARTING_AREA_SIZE = 3;
    public const int DAYS_PER_WEEK = 5;
    public const int WEEKS_PER_MONTH = 4;
    public const int MONTHS_PER_YEAR = 12;
    public const int DAYS_PER_MONTH = DAYS_PER_WEEK * WEEKS_PER_MONTH;
    public const int DAYS_PER_YEAR = MONTHS_PER_YEAR * DAYS_PER_MONTH;
    public const int CLAIMS_NEEDED_TO_ACQUIRE_TILES = 5;
    public const int CUSTOMER_ORDER_MISSES_IN_A_ROW_TO_LOSE_GAME = 2;

    // State
    public GameState GameState { get; private set; }
    public int Day { get; private set; }
    public Map Map { get; private set; }

    // Production
    public ResourceCollection Resources { get; private set; }
    public List<Object> Objects { get; private set; }
    public Dictionary<ResourceDef, ResourceProduction> CurrentFinalResourceProduction { get; private set; }
    public Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> CurrentPerTileResourceProduction { get; private set; }

    // Orders
    public List<Customer> WeeklyCustomers { get; private set; }
    public List<Order> ActiveOrders { get; private set; }
    public Customer TownCouncil { get; private set; }
    public List<TownMandate> TownMandates { get; private set; }
    public TownMandate NextTownMandate => (IsLastDayOfMonth && Game.Instance.GameState > GameState.ConfirmedScatter) ? TownMandates[Month + 1] : TownMandates[Month];

    // Visual
    public bool IsShowingGridOverlay { get; private set; }


    #region Initialization

    public Game()
    {
        Instance = this;
        Day = 1;
        Map = MapGenerator.GenerateMap(21);
        CurrentFinalResourceProduction = new Dictionary<ResourceDef, ResourceProduction>();
        CurrentPerTileResourceProduction = new Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>>();

        // Initialize resources
        Resources = new ResourceCollection();
        foreach (ResourceDef def in DefDatabase<ResourceDef>.AllDefs.Where(r => r.Type == ResourceType.MarketResource)) Resources.Resources.Add(def, 0);

        // Starting garden area
        int gardenStartAreaSize = 3;
        int half = gardenStartAreaSize / 2;
        for (int x = -half; x <= half; x++)
        {
            for (int y = -half; y <= half; y++)
            {
                AddTileToGarden(Map.GetTile(x, y), redraw: false);
            }
        }

        // Starting objects
        Objects = new List<Object>();
        AddObjectToInventory(ObjectDefOf.Carrot);

        // Weekly orders
        WeeklyCustomers = new List<Customer>();
        AddOrUpgradeCustomer(DefDatabase<CustomerDef>.GetNamed("TownCanteen"));
        ActiveOrders = new List<Order>();
        CreateNextWeeksOrders(isFirstWeekOrder: true);

        // Town mandates
        TownCouncil = new Customer(CustomerDefOf.TownCouncil, 1);
        TownMandates = new List<TownMandate>();
        for (int i = 0; i < MONTHS_PER_YEAR; i++)
        {
            TownMandates.Add(new TownMandate(i, DefDatabase<TownMandateDef>.AllDefs[i]));
        }

        GameState = GameState.Uninitialized;
    }

    public void Initialize()
    {
        // Render
        IsShowingGridOverlay = true;
        DrawFullMap();
        CameraHandler.Instance.SetBounds(Map.MinX, Map.MinY, Map.MaxX, Map.MaxY);
        CameraHandler.Instance.FocusPosition(new Vector2(0f, 0f));

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

        // Dev mode - Terrain
        if (UI_DevModePanel.Instance.IsChangeTerrainActive)
        {
            // Compute which cell the mouse is over
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = MapRenderer.Instance.ObjectTilemap.WorldToCell(worldPos);
            MapTile currentTile = Game.Instance.Map.GetTile(cell.x, cell.y);

            if (currentTile != null && !HelperFunctions.IsMouseOverUi())
            {
                int currentTerrainIndex = DefDatabase<TerrainDef>.AllDefs.IndexOf(currentTile.Terrain.Def);
                if (Input.GetMouseButtonDown(0))
                {
                    int nextIndex = currentTerrainIndex + 1;
                    if (nextIndex == DefDatabase<TerrainDef>.AllDefs.Count) nextIndex = 0;
                    TerrainDef newTerrain = DefDatabase<TerrainDef>.AllDefs[nextIndex];
                    SetTerrain(currentTile.Coordinates, newTerrain);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    int prevIndex = currentTerrainIndex - 1;
                    if (prevIndex == -1) prevIndex = DefDatabase<TerrainDef>.AllDefs.Count - 1;
                    TerrainDef newTerrain = DefDatabase<TerrainDef>.AllDefs[prevIndex];
                    SetTerrain(currentTile.Coordinates, newTerrain);
                }
            }
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
        CurrentFinalResourceProduction = GetCurrentScatterProduction();

        ApplyMarketResources();
        ApplyAbstractResources();

        // State
        GameState = GameState.ConfirmedScatter;

        // UI
        DrawFullMap();
        GameUI.Instance.ResourcePanel.Refresh();
        NestedTooltipManager.Instance.ResetTooltips();
    }

    /// <summary>
    /// Adds all market resources produced today to the resource pool.
    /// </summary>
    private void ApplyMarketResources()
    {
        ResourceCollection marketResourcesToAdd = new ResourceCollection();
        foreach (var kvp in CurrentFinalResourceProduction)
        {
            ResourceDef resource = kvp.Key;
            ResourceProduction production = kvp.Value;

            if (resource.Type == ResourceType.MarketResource)
            {
                marketResourcesToAdd.AddResource(resource, production.GetValue());
            }
        }
        AddResources(marketResourcesToAdd);
    }

    /// <summary>
    /// Applies the effect of all abstract resources that were produced today.
    /// </summary>
    private void ApplyAbstractResources()
    {
        foreach (var kvp in CurrentPerTileResourceProduction)
        {
            MapTile tile = kvp.Key;
            Dictionary<ResourceDef, ResourceProduction> tileProduction = kvp.Value;

            // Fertility
            if (tileProduction.TryGetValue(ResourceDefOf.Fertility, out ResourceProduction fertilityProduction) && fertilityProduction.GetValue() != 0)
            {
                tile.Terrain.AdjustFertility(fertilityProduction.GetValue());
            }

            // Expansion
            if (tileProduction.TryGetValue(ResourceDefOf.Expansion, out ResourceProduction expansionProduction) && expansionProduction.GetValue() != 0)
            {
                int expansion = expansionProduction.GetValue();
                foreach(MapTile adjTile in tile.GetOrthogonalAdjacentTiles().Where(t => !t.IsOwned))
                {
                    adjTile.AdjustClaim(expansion);
                }
            }
        }
    }

    private void StartPostScatter()
    {
        if (IsLastDayOfMonth)
        {
            // Pay town mandate
            if (Resources.HasResources(NextTownMandate.OrderedResources))
            {
                Resources.RemoveResources(NextTownMandate.OrderedResources);

                // UI
                GameUI.Instance.ResourcePanel.Refresh();
                GameUI.Instance.OrderPanel.Refresh();

                if (IsLastDayOfYear)
                {
                    WinGame();
                    return;
                }
            }
            else
            {
                LoseGame();
                return;
            }
        }

        if (IsLastDayOfWeek)
        {
            StartOrderSelection();
        }
        else
        {
            string title = $"Day {Day} Complete";
            StartObjectDraft(title, ObjectTierDefOf.Common);
        }
        
    }

    /// <summary>
    /// Removes all orders that are due today.
    /// Delivers the resources to the ones selected and applies punishments if not.
    /// </summary>
    private void HandleDueOrders(List<Order> deliveredOrders)
    {
        foreach (Order order in DueOrders)
        {
            if (deliveredOrders.Contains(order))
            {
                Resources.RemoveResources(order.OrderedResources);
                order.Customer.ResetMissedOrders();
            }
            else
            {
                order.Customer.IncrementMissedOrders();
                if(order.Customer.MissedOrders >= CUSTOMER_ORDER_MISSES_IN_A_ROW_TO_LOSE_GAME)
                {
                    LoseGame();
                }
            }
        }

        ActiveOrders = ActiveOrders.Where(o => o.DueDay != Day).ToList();
    }

    /// <summary>
    /// Upgrades the level of an existing customer or creates adds a new customer.
    /// </summary>
    private void UpgradeRandomCustomer()
    {
        CustomerDef chosenDef = DefDatabase<CustomerDef>.AllDefs.Where(c => c.IsWeeklyCustomer).ToList().RandomElement();
        AddOrUpgradeCustomer(chosenDef);
    }

    /// <summary>
    /// Creates next weeks orders according to current customers.
    /// </summary>
    private void CreateNextWeeksOrders(bool isFirstWeekOrder)
    {
        foreach (Customer customer in WeeklyCustomers)
        {
            ResourceCollection customerOrder = customer.GetCurrentLevelOrder();
            int targetWeek = isFirstWeekOrder ? 1 : GetWeekNumber() + 1;
            ActiveOrders.Add(new Order(customer, targetWeek * DAYS_PER_WEEK, customerOrder));
        }
    }

    /// <summary>
    /// Calculates and returns the final resource productions of the day, including abstract resources.
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

        // Apply modifiers from each tile to all other affected objects
        foreach (MapTile tile in Map.OwnedTiles)
        {
            foreach (ObjectEffect effect in tile.GetEffects())
            {
                if (!effect.Validate(out string invalidReason)) throw new System.Exception($"Cannot apply invalid effect of {tile}. ValidationFailReason: {invalidReason}");
                effect.ApplyEffectTo(tile, CurrentPerTileResourceProduction);
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
        Debug.Log($"Starting Day {Day}.");

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

    #region Draft

    private void StartObjectDraft(string title, ObjectTierDef tier)
    {
        GameState = GameState.ObjectDraft;

        // Get options
        List<ObjectDef> draftOptions = GetDraftOptions(tier);

        // Show draft window
        string draftWindowTitle = title;
        string draftWindowSubtitle = $"Choose a {tier.Label} object to add to your inventory";
        UI_ObjectDraftWindow.Instance.ShowObjectDraft(draftWindowTitle, draftWindowSubtitle, draftOptions, OnObjectDrafted);
    }

    private void OnObjectDrafted(List<IDraftable> selectedOptions)
    {
        // Add drafted objects to inventory
        foreach (IDraftable draftedObject in selectedOptions)
        {
            ObjectDef def = (ObjectDef)draftedObject;
            AddObjectToInventory(def);
        }

        // End day
        EndDay();
    }

    private List<ObjectDef> GetDraftOptions(ObjectTierDef tier)
    {
        // Create candidate probability table
        Dictionary<ObjectDef, float> candidates = new Dictionary<ObjectDef, float>();
        foreach (ObjectDef def in DefDatabase<ObjectDef>.AllDefs)
        {
            if (def.Tier == tier) candidates.Add(def, 1f);
        }

        // Choose draft options out of candidates
        List<ObjectDef> draftOptions = candidates.GetWeightedRandomElements(amount: 3, allowRepeating: false);
        return draftOptions;
    }


    /// <summary>
    /// Shows the window where the player can select which orders to fulfill.
    /// </summary>
    private void StartOrderSelection()
    {
        GameState = GameState.OrderSelection;

        // Get options
        List<Order> orderOptions = DueOrders;

        // Show draft window
        string draftWindowTitle = $"Week {GetWeekNumber()} Complete";
        string draftWindowSubtitle = "Select the orders you want to deliver.";
        if (IsLastDayOfMonth) draftWindowSubtitle += "\nThe town mandate has already been payed.";
        UI_OrderSelectionWindow.Instance.ShowOrderSelection(draftWindowTitle, draftWindowSubtitle, orderOptions, OnOrdersSelected);
    }

    private void OnOrdersSelected(List<IDraftable> selectedOptions)
    {
        // Order logic
        List<Order> deliveredOrders = selectedOptions.Select(o => (Order)o).ToList();
        HandleDueOrders(deliveredOrders);
        if (GameState == GameState.GameOver) return;
        UpgradeRandomCustomer();
        CreateNextWeeksOrders(isFirstWeekOrder: false);

        // UI
        GameUI.Instance.ResourcePanel.Refresh();
        GameUI.Instance.OrderPanel.Refresh();

        // Next step
        if (IsLastDayOfMonth)
        {
            string title = $"{CurrentMonthName} Complete";
            StartObjectDraft(title, ObjectTierDefOf.Epic);
        }
        else
        {
            string title = $"Week {GetWeekNumber()} Complete";
            StartObjectDraft(title, ObjectTierDefOf.Rare);
        }
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

    public void AddOrUpgradeCustomer(CustomerDef def)
    {
        Customer existingCustomer = WeeklyCustomers.FirstOrDefault(c => c.Def == def);
        if (existingCustomer != null) existingCustomer.IncreaseLevel();
        else WeeklyCustomers.Add(new Customer(def, orderLevel: 1));
    }

    public void SetTerrain(Vector2Int coordinates, TerrainDef newTerrain)
    {
        Map.SetTerrain(coordinates, newTerrain);
        DrawFullMap();
    }

    private void WinGame()
    {
        GameState = GameState.GameOver;
        UI_GameOverWindow.Instance.Show("You Win!");
    }
    private void LoseGame()
    {
        GameState = GameState.GameOver;
        UI_GameOverWindow.Instance.Show("You Lose.");
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

    public string GetMonthName(int monthIndex)
    {
        string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        return monthNames[monthIndex];
    }
    public string CurrentMonthName => GetMonthName(Month);

    public int GetWeekNumber()
    {
        return ((Day - 1) / DAYS_PER_WEEK) + 1;
    }

    public int Month => (Day - 1) / DAYS_PER_MONTH;

    public bool IsLastDayOfWeek => (Day - 1) % DAYS_PER_WEEK == DAYS_PER_WEEK - 1;
    public bool IsLastDayOfMonth => (Day - 1) % DAYS_PER_MONTH == DAYS_PER_MONTH - 1;
    public bool IsLastDayOfYear => (Day - 1) % DAYS_PER_YEAR == DAYS_PER_YEAR - 1;

    public Dictionary<ResourceDef, ResourceProduction> GetTileProduction(MapTile tile)
    {
        if (CurrentPerTileResourceProduction.TryGetValue(tile, out var prod)) return prod;
        return null;
    }

    public List<Order> DueOrders => ActiveOrders.Where(o => o.DueDay == Day).ToList();

    #endregion
}
