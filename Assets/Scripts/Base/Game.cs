using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Game
{
    public static Game Instance;

    // Rules
    public const int STARTING_AREA_SIZE = 3;
    public const int DAYS_PER_WEEK = 5;
    public const int WEEKS_PER_MONTH = 4;
    public const int MONTHS_PER_YEAR = 12;
    public const int DAYS_PER_MONTH = DAYS_PER_WEEK * WEEKS_PER_MONTH;
    public const int DAYS_PER_YEAR = MONTHS_PER_YEAR * DAYS_PER_MONTH;
    public const int CUSTOMER_ORDER_MISSES_IN_A_ROW_TO_LOSE_GAME = 2;

    public const int FIRST_TILE_RING_COST = 10;
    public const float COST_INCREASE_PER_TILE_RING = 2f;

    public const float RESOURCE_SELL_VALUE = 0.5f;
    public const float RESOURCE_BUY_PRICE = 2f;
    public const float SHOP_DISCOUNT = 0.50f; // in % of original price

    private const int INITIAL_SHED_CABINETS = 1;
    private const int INITIAL_SHED_CABINET_SHELVES = 3;
    private const int INITIAL_SHED_CABINET_SHELF_OBJECTS = 3;


    // State
    public GameState GameState { get; private set; }
    public int Day { get; private set; }
    public Map Map { get; private set; }
    public bool IsAcquiringTiles { get; private set; }

    // Sectors
    public List<GardenSector> Sectors;

    // Production
    public ResourceCollection Resources { get; private set; }
    public List<Object> AllObjects { get; private set; }
    public Dictionary<ResourceDef, ResourceProduction> CurrentFinalResourceProduction { get; private set; }
    public Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> CurrentPerTileResourceProduction { get; private set; }

    // Orders
    public List<Customer> WeeklyCustomers { get; private set; }
    public List<Order> ActiveOrders { get; private set; }
    public Customer TownCouncil { get; private set; }
    public List<TownMandate> TownMandates { get; private set; }
    public TownMandate NextTownMandate => (IsLastDayOfMonth && Game.Instance.GameState > GameState.Evening_HarvestAnimation) ? TownMandates[Month + 1] : TownMandates[Month];

    // Shop
    public ResourceCollection ShopResources { get; private set; }
    public Dictionary<ObjectDef, int> ShopObjects { get; private set; }
    public ObjectDef ShopDiscountedObject { get; private set; }

    // Visual
    public bool IsShowingGridOverlay { get; private set; }

    // Inputs
    private MapTile HoveredTile;

    // UI
    private UI_HUD HUD;


    #region Initialization

    public Game()
    {
        Instance = this;
        Day = 0;
        CurrentFinalResourceProduction = new Dictionary<ResourceDef, ResourceProduction>();
        CurrentPerTileResourceProduction = new Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>>();

        // Generate map
        Map = MapGenerator.GenerateMap(23);
        Map.GetTile(0, 0).Acquire();

        // References
        HUD = GameObject.Find("HUD").GetComponent<UI_HUD>();
        if (HUD == null) throw new System.Exception("Couldn't find HUD");

        // Initialize resources
        Resources = new ResourceCollection();
        foreach (ResourceDef def in DefDatabase<ResourceDef>.AllDefs.Where(r => r.Type == ResourceType.Currency || r.Type == ResourceType.MarketResource)) Resources.Resources.Add(def, 0);

        // Starting sector
        Sectors = new List<GardenSector>();
        AddSector(new Vector2Int(0, 0));

        int gardenStartAreaSize = 3;
        int half = gardenStartAreaSize / 2;
        for (int x = -half; x <= half; x++)
        {
            for (int y = -half; y <= half; y++)
            {
                AddTileToGarden(Sectors[0], Map.GetTile(x, y), redraw: false);
            }
        }

        // Starting objects
        AllObjects = new List<Object>();
        AddNewObjectToInventory(Sectors[0], ObjectDefOf.Carrot);

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
        IsShowingGridOverlay = false;
        DrawFullMap();
        CameraHandler.Instance.SetBounds(Map.MinX, Map.MinY, Map.MaxX, Map.MaxY);
        CameraHandler.Instance.FocusPosition(new Vector2(0f, 0f));

        // UI
        HUD.AcquireTilesControl.OnToggle += SetAcquireTilesMode;
        UI_TileOverlayContainer.Instance.InitOverlays();

        StartNewDay();
    }

    #endregion

    #region Game Loop

    public void AdvanceGameLoop()
    {
        if (GameState == GameState.Morning) StartScatter();
        else if (GameState == GameState.Afternoon) StartHarvest();
        else if (GameState == GameState.Evening_PostHarvest) StartPostScatter();
    }

    public void Update()
    {
        // Update Hover
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = MapRenderer.Instance.ObjectTilemap.WorldToCell(worldPos);
        HoveredTile = Game.Instance.Map.GetTile(cell.x, cell.y);

        // Buy Tiles
        if (IsAcquiringTiles)
        {
            if (HoveredTile != null && !HelperFunctions.IsMouseOverUi())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    TryToBuyTile(Sectors[0], HoveredTile);
                }
            }
        }

        // Shed window
        if (Input.GetMouseButtonDown(0))
        {
            if (!HelperFunctions.IsMouseOverUi())
            {
                if (HoveredTile != null && HoveredTile.HasObject && HoveredTile.Object.Def == ObjectDefOf.Shed)
                {
                    Shed_OnClick(HoveredTile);
                }
            }
        }

        // Game Loop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceGameLoop();
        }

        switch( GameState)
        {
            case GameState.Noon:
                ScatterAnimationManager.Update();
                break;

            case GameState.Evening_HarvestAnimation:
                HarvestAnimationManager.Update();
                break;
        }

        UpdateDevModeInputs();
    }

    private void UpdateDevModeInputs()
    {
        // Terrain
        if (UI_DevModePanel.Instance.IsChangeTerrainActive)
        {
            if (HoveredTile != null && !HelperFunctions.IsMouseOverUi())
            {
                int currentTerrainIndex = DefDatabase<TerrainDef>.AllDefs.IndexOf(HoveredTile.Terrain.Def);
                if (Input.GetMouseButtonDown(0))
                {
                    int nextIndex = currentTerrainIndex + 1;
                    if (nextIndex == DefDatabase<TerrainDef>.AllDefs.Count) nextIndex = 0;
                    TerrainDef newTerrain = DefDatabase<TerrainDef>.AllDefs[nextIndex];
                    SetTerrain(HoveredTile.Coordinates, newTerrain);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    int prevIndex = currentTerrainIndex - 1;
                    if (prevIndex == -1) prevIndex = DefDatabase<TerrainDef>.AllDefs.Count - 1;
                    TerrainDef newTerrain = DefDatabase<TerrainDef>.AllDefs[prevIndex];
                    SetTerrain(HoveredTile.Coordinates, newTerrain);
                }
            }
        }

        // Ownership
        if (UI_DevModePanel.Instance.IsToggleOwnershipActive)
        {
            if (HoveredTile != null && !HelperFunctions.IsMouseOverUi())
            {
                if (Input.GetMouseButtonDown(0)) AddTileToGarden(Sectors[0], HoveredTile);
                if (Input.GetMouseButtonDown(1)) RemoveTileFromGarden(HoveredTile);
            }
        }
    }

    /// <summary>
    /// Scatters the objects around the garden.
    /// </summary>
    public void StartScatter()
    {
        GameState = GameState.Noon;

        HUD.RefreshGameLoopButton();

        foreach (GardenSector sector in Sectors) sector.InitScatter();
        ScatterAnimationManager.StartAnimation(callback: OnScatterAnimationDone);
    }

    public void OnObjectArrivedDuringScatter(Object obj, MapTile tile)
    {
        Game.Instance.PlaceObject(obj, tile);
        
        Game.Instance.DrawFullMap();

        CurrentFinalResourceProduction = GetCurrentScatterProduction();
        HUD.ResourcePanel.Refresh();
    }

    private void OnScatterAnimationDone()
    {
        GameState = GameState.Afternoon;
        DrawFullMap(); // To close doors

        // UI
        HUD.DatePanel.Refresh();
        HUD.ResourcePanel.Refresh();
        TooltipManager.Instance.ResetTooltips();
        HUD.AcquireTilesControl.Hide();
        HUD.ShopControl.transform.parent.gameObject.SetActive(false);

        HUD.RefreshGameLoopButton();
    }

    private void StartHarvest()
    {
        // State
        GameState = GameState.Evening_HarvestAnimation;

        // Calculate final production of the day
        CurrentFinalResourceProduction = GetCurrentScatterProduction();

        // Start animation
        HarvestAnimationManager.StartAnimation(callback: OnHarvestAnimationDone);
        HUD.RefreshGameLoopButton();

        // todo: migrate this to animation

        DecrementModifierDurations();
        ApplyNewModifiers();

        // todo end
    }

    public void OnResourceIconArrivesDuringHarvest(MapTile sourceTile, ResourceDef res, bool isNegative)
    {
        if (res.Type == ResourceType.MarketResource || res.Type == ResourceType.Currency)
        {
            if (isNegative) HarvestAnimationManager.CurrentResourcePlusValues[res]++;
            else HarvestAnimationManager.CurrentResourcePlusValues[res]--;
            AddSingleAccumulativeResource(res);
        }
        else if (res.Type == ResourceType.TerrainModificationResource)
        {
            if (res == ResourceDefOf.Fertility)
            {
                sourceTile.Terrain.AdjustFertility(isNegative? -1 : 1);
                DrawFullMap();
            }
        }
    }

    public void OnObjectAppliesModifierDuringHarvest()
    {

    }

    public void OnObjectReturnedDuringHarvest()
    {

    }

    private void OnHarvestAnimationDone()
    {
        // State
        GameState = GameState.Evening_PostHarvest;

        // UI
        DrawFullMap();
        HUD.DatePanel.Refresh();
        HUD.ResourcePanel.Refresh();
        TooltipManager.Instance.ResetTooltips();

        HUD.RefreshGameLoopButton();
    }


    /// <summary>
    /// Decrements the duration of all modifiers.
    /// </summary>
    private void DecrementModifierDurations()
    {
        foreach (MapTile tile in Map.OwnedTiles)
        {
            tile.DecrementModifierDurations();
            if(tile.HasObject)
            {
                tile.Object.DecrementModifierDurations();
            }
        }
    }

    /// <summary>
    /// Applies all modifiers that got added to objects or tiles from effects this turn.
    /// </summary>
    private void ApplyNewModifiers()
    {
        foreach (MapTile tile in Map.OwnedTiles)
        {
            foreach (ObjectEffect effect in tile.GetEffects())
            {
                effect.ApplyObjectModifiers(tile);
            }
        }
    }

    private void StartPostScatter()
    {
        if (IsShedWindowOpen) return; // Can't go to night when shed window is open

        // Remove all objects from day
        Map.ClearAllNonPermanentObjects();
        DrawFullMap();

        if (IsLastDayOfMonth)
        {
            // Pay town mandate
            if (Resources.HasResources(NextTownMandate.OrderedResources))
            {
                Resources.RemoveResources(NextTownMandate.OrderedResources);

                // UI
                HUD.DatePanel.Refresh();
                HUD.ResourcePanel.Refresh();
                HUD.OrderPanel.Refresh();

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

        // UI
        HUD.RefreshGameLoopButton();
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
                Resources.AddResources(order.Reward);
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
            ResourceCollection reward = customer.GetCurrentLevelReward();
            int targetWeek = isFirstWeekOrder ? 1 : GetWeekNumber() + 1;
            ActiveOrders.Add(new Order(customer, targetWeek * DAYS_PER_WEEK, customerOrder, reward));
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
            string label = tile.HasObject ? tile.Object.LabelCapWord : "Empty Tile";

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
                effect.ApplyProductionModifiers(tile, CurrentPerTileResourceProduction);
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
                    finalProduction[resource].AddProductionModifier(new ProductionModifier(production.Label, ProductionModifierType.Additive, production.GetValue()));
                }
            }
        }

        return finalProduction;
    }

    public void EndDay()
    {
        CurrentFinalResourceProduction.Clear();

        StartNewDay();
    }

    #endregion

    #region Morning

    private void StartNewDay()
    {
        Day++;
        Debug.Log($"Starting Day {Day}.");
        if (Day % DAYS_PER_MONTH == 1) StartNewMonth();

        GameState = GameState.Morning;

        // UI
        HUD.DatePanel.Refresh();
        HUD.ResourcePanel.Refresh();
        HUD.OrderPanel.Refresh();
        TooltipManager.Instance.ResetTooltips();
        HUD.AcquireTilesControl.Show();
        HUD.ShopControl.transform.parent.gameObject.SetActive(true);

        HUD.RefreshGameLoopButton();
    }

    private void StartNewMonth()
    {
        Debug.Log($"Starting Month {CurrentMonthName}.");

        // Shop restock
        ShopResources = new ResourceCollection(ResourceType.MarketResource);
        foreach (ResourceDef res in ShopResources.Resources.Keys.ToList())
        {
            ShopResources.AddResource(res, Random.Range(0, 4) * 10);
        } 

        ShopObjects = new Dictionary<ObjectDef, int>();
        Dictionary<ObjectTierDef, int> numObjectsByTier = new Dictionary<ObjectTierDef, int>()
        {
            { ObjectTierDefOf.Common, 3 },
            { ObjectTierDefOf.Rare, 2 },
            { ObjectTierDefOf.Epic, 1 },
        };

        foreach(var tier in numObjectsByTier)
        {
            List<ObjectDef> commonObjects = DefDatabase<ObjectDef>.AllDefs.Where(x => x.Tier == tier.Key).ToList().RandomElements(tier.Value);
            foreach (ObjectDef obj in commonObjects) ShopObjects.Add(obj, tier.Key.MarketValue);
        }
        ShopDiscountedObject = ShopObjects.Keys.ToList().RandomElement();
        ShopObjects[ShopDiscountedObject] = (int)(ShopObjects[ShopDiscountedObject] * SHOP_DISCOUNT);
    }

    public void SetAcquireTilesMode(bool value)
    {
        IsAcquiringTiles = value;
        RefreshAcquiringTilesOverlays();
    }

    private void RefreshAcquiringTilesOverlays()
    {
        if (IsAcquiringTiles) UI_TileOverlayContainer.Instance.ShowTileCostOverlay();
        else UI_TileOverlayContainer.Instance.HideAllOverlays();
    }

    private void TryToBuyTile(GardenSector sector, MapTile tile)
    {
        if (!Resources.HasResources(tile.AcquireCost)) return; // not enough money

        Resources.RemoveResources(tile.AcquireCost);
        AddTileToGarden(sector, tile);

        // UI
        HUD.ResourcePanel.Refresh();
        RefreshAcquiringTilesOverlays();
    }


    #endregion

    #region Draft

    private void StartObjectDraft(string title, ObjectTierDef tier)
    {
        GameState = GameState.Night_ObjectDraft;

        // Get options
        List<ObjectDef> draftOptions = GetDraftOptions(tier);

        // Show draft window
        string draftWindowTitle = title;
        string draftWindowSubtitle = $"Choose a {tier.Label} object to add to your inventory";
        UI_ObjectDraftWindow.Instance.ShowObjectDraft(draftWindowTitle, draftWindowSubtitle, draftOptions, OnObjectDrafted);

        // UI
        HUD.DatePanel.Refresh();
    }

    private void OnObjectDrafted(List<IDraftable> selectedOptions)
    {
        // Add drafted objects to inventory
        foreach (IDraftable draftedObject in selectedOptions)
        {
            ObjectDef def = (ObjectDef)draftedObject;
            AddNewObjectToInventory(Sectors[0], def);
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
        GameState = GameState.Night_OrderSelection;

        // Get options
        List<Order> orderOptions = DueOrders;

        // Show draft window
        string draftWindowTitle = $"Week {GetWeekNumber()} Complete";
        string draftWindowSubtitle = "Select the orders you want to deliver.";
        if (IsLastDayOfMonth) draftWindowSubtitle += "\nThe town mandate has already been payed.";
        UI_OrderSelectionWindow.Instance.ShowOrderSelection(draftWindowTitle, draftWindowSubtitle, orderOptions, OnOrdersSelected);


        // UI
        HUD.DatePanel.Refresh();
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
        HUD.ResourcePanel.Refresh();
        HUD.OrderPanel.Refresh();

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

    public void PlaceObject(Object obj, MapTile tile)
    {
        tile.SetObject(obj);
        obj.SetTile(tile);
    }

    /// <summary>
    /// Removes an object from a tile.
    /// </summary>
    public void ClearTile(MapTile tile)
    {
        if (tile.HasObject) tile.Object.SetTile(null);
        tile.SetObject(null);
    }

    public void AddSector(Vector2Int shedPosition)
    {
        MapTile initialTile = Map.GetTile(shedPosition);
        if (!initialTile.IsOwned) throw new System.Exception("Can't create a sector on an unowned tile.");

        GardenSector newSector = new GardenSector("Starting Sector", initialTile);
        Object shed = new Object(ObjectDefOf.Shed);
        PlaceObject(shed, initialTile);
        initialTile.SetSector(newSector);
        newSector.AddTile(initialTile);

        Sectors.Add(newSector);
    }

    public void AddNewObjectToInventory(GardenSector sector, ObjectDef def)
    {
        Object newObj = new Object(def);
        AllObjects.Add(newObj);
        sector.AddObject(newObj);
    }

    public void AddTileToGarden(GardenSector sector, MapTile tile, bool redraw = true)
    {
        tile.Acquire();
        tile.SetSector(sector);
        sector.AddTile(tile);
        if (redraw) DrawFullMap();
    }
    public void RemoveTileFromGarden(MapTile tile, bool redraw = true)
    {
        tile.Unacquire();
        if (redraw) DrawFullMap();
    }

    public void AddSingleAccumulativeResource(ResourceDef def)
    {
        Resources.AddResource(def, 1);

        // UI
        HUD.ResourcePanel.Refresh();
        if (IsAcquiringTiles) RefreshAcquiringTilesOverlays();
    }
    public void AddResources(ResourceCollection res)
    {
        Resources.AddResources(res);

        // UI
        HUD.ResourcePanel.Refresh();
        if (IsAcquiringTiles) RefreshAcquiringTilesOverlays();
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

    public void CompleteTrade()
    {
        // Resources
        Resources.RemoveResources(UI_ShopWindow.Instance.Selling.Resources);
        Resources.AddResources(UI_ShopWindow.Instance.Buying.Resources);

        ShopResources.RemoveResources(UI_ShopWindow.Instance.Buying.Resources);
        ShopResources.AddResources(UI_ShopWindow.Instance.Selling.Resources);

        // Objects
        foreach (ObjectDef obj in UI_ShopWindow.Instance.Buying.Objects.Keys)
        {
            AddNewObjectToInventory(Sectors[0], obj);
            ShopObjects.Remove(obj);
        }

        // Payment
        if (UI_ShopWindow.Instance.CurrentFinalBalance < 0) Resources.RemoveResource(ResourceDefOf.Gold, Mathf.Abs(UI_ShopWindow.Instance.CurrentFinalBalance));
        else Resources.AddResource(ResourceDefOf.Gold, UI_ShopWindow.Instance.CurrentFinalBalance);

        // UI
        HUD.ResourcePanel.Refresh();
    }

    #endregion

    #region Shed Window

    public void Shed_OnClick(MapTile tile)
    {
        if (UI_ShedWindow.Instance.gameObject.activeSelf)
        {
            if (UI_ShedWindow.Instance.DisplayedSector == tile.Sector) CloseShed(tile.Sector);
            else OpenShed(tile.Sector);
        }
        else
        {
            OpenShed(tile.Sector);
        }
    }

    public void CloseShed(GardenSector sector)
    {
        UI_ShedWindow.Instance.Hide();
        DrawFullMap();
        HUD.RefreshGameLoopButton();
    }

    private void OpenShed(GardenSector sector)
    {
        if (!CanOpenShedWindow()) return;

        UI_ShedWindow.Instance.Show(sector);
        DrawFullMap();
        HUD.RefreshGameLoopButton();
    }

    public bool CanOpenShedWindow()
    {
        if (IsNight) return false;
        return true;
    }

    public bool IsShedWindowOpen => UI_ShedWindow.Instance.gameObject.activeSelf;

    #endregion

    #region Render

    public void DrawFullMap()
    {
        MapRenderer.Instance.DrawFullMap(Map);
    }

    #endregion

    #region Getters

    public bool IsNight => GameState == GameState.Night_ObjectDraft || GameState == GameState.Night_OrderSelection;

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

    public string GetCurrentTimeOfDay()
    {
        if (GameState == GameState.Morning) return "Morning";
        if (GameState == GameState.Noon) return "Noon";
        if (GameState == GameState.Afternoon) return "Afternoon";
        if (GameState == GameState.Evening_HarvestAnimation) return "Evening";
        return "Night";
    }

    public int GetWeekNumber()
    {
        return ((Day - 1) / DAYS_PER_WEEK) + 1;
    }

    public int Month => (Day - 1) / DAYS_PER_MONTH;

    public bool IsLastDayOfWeek => (Day - 1) % DAYS_PER_WEEK == DAYS_PER_WEEK - 1;
    public bool IsLastDayOfMonth => (Day - 1) % DAYS_PER_MONTH == DAYS_PER_MONTH - 1;
    public bool IsLastDayOfYear => (Day - 1) % DAYS_PER_YEAR == DAYS_PER_YEAR - 1;

    public int DaysUntilNextMonth()
    {
        int nextMandateDueDay = NextTownMandate.DueDay;
        return nextMandateDueDay - Day + 1;
    }

    public Dictionary<ResourceDef, ResourceProduction> GetTileProduction(MapTile tile)
    {
        if (CurrentPerTileResourceProduction.TryGetValue(tile, out var prod)) return prod;
        return null;
    }

    public List<Order> DueOrders => ActiveOrders.Where(o => o.DueDay == Day).ToList();

    public int GetShedCabinetAmount()
    {
        return INITIAL_SHED_CABINETS;
    }
    public int GetShedCabinetShelfAmount()
    {
        return INITIAL_SHED_CABINET_SHELVES;
    }
    public int GetShedCabinetShelfObjectAmount()
    {
        return INITIAL_SHED_CABINET_SHELF_OBJECTS;
    }

    #endregion
}
