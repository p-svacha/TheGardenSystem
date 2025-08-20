using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HarvestAnimationManager
{
    private const float START_DELAY = 0.7f;
    private const float RESOURCE_ICON_INTERVAL = 0.35f;
    public const float RESOURCE_ICON_FYLING_SPEED = 1000f;
    private const float OBJECT_INTERVAL = 0.35f;
    private const float END_DELAY = 0.7f;

    private const float MODIFIER_INTERVAL = 0.08f;             // time between spawn waves
    private const float MODIFIER_FIZZLE_DURATION = 0.45f;      // each fizzle’s expand+fade duration

    public static HarvestAnimationState State;
    private static float Timer;
    private static float LastStateEventTime;
    private static int StateIndex;
    private static int MaxResourcesProduced;
    private static List<GardenSector> Sectors;
    private static List<FlyingResourceIcon> FlyingResourceIcons;
    private static List<FlyingObjectSprite> ReturningObjects;
    private static List<FizzlingModifier> FizzlingModifiers = new List<FizzlingModifier>();

    private static System.Action Callback;

    public static Dictionary<ResourceDef, int> CurrentResourcePlusValues;

    public static void StartAnimation(System.Action callback)
    {
        Callback = callback;

        // Reset values
        Timer = 0f;
        LastStateEventTime = 0f;
        Sectors = new List<GardenSector>(Game.Instance.Sectors);
        StateIndex = 0;
        FlyingResourceIcons = new List<FlyingResourceIcon>();
        ReturningObjects = new List<FlyingObjectSprite>();
        State = HarvestAnimationState.InitialWait;

        CurrentResourcePlusValues = new Dictionary<ResourceDef, int>();
        foreach (var kvp in Game.Instance.CurrentFinalResourceProduction)
            CurrentResourcePlusValues.Add(kvp.Key, kvp.Value.GetValue());

        // Open all shed doors
        Game.Instance.DrawFullMap();
    }

    public static void Update()
    {
        Timer += Time.deltaTime;

        switch (State)
        {
            case HarvestAnimationState.InitialWait:
                UpdateInitialWait();
                break;

            case HarvestAnimationState.ResourceDistribution:
                UpdateResourceDistribution();
                break;

            case HarvestAnimationState.ExistingModifierTicker:
                UpdateExistingModifierTicker();
                break;

            case HarvestAnimationState.NewModifierApplication:
                UpdateNewModifierApplication();
                break;

            case HarvestAnimationState.ObjectsReturning:
                UpdateObjectsReturning();
                break;

            case HarvestAnimationState.FinalWait:
                UpdateFinalWait();
                break;
        }
    }

    private static void SwitchStateTo(HarvestAnimationState state)
    {
        Debug.Log($"Starting harvest animation state: {state} after {Timer} seconds.");
        State = state;

        Timer = 0f;
        LastStateEventTime = 0f;
        StateIndex = 0;
    }

    private static void UpdateInitialWait()
    {
        if (Timer >= START_DELAY)
        {
            MaxResourcesProduced = Sectors.Max(s => s.Objects.Where(o => o.Tile != null).Max(o => Game.Instance.GetTileProduction(o.Tile).Sum(p => p.Value.GetValue())));
            Debug.Log($"Max resources produced this day is {MaxResourcesProduced}");

            SwitchStateTo(HarvestAnimationState.ResourceDistribution);
        }
    }

    private static void UpdateResourceDistribution()
    {
        // Flying resource icons
        foreach (FlyingResourceIcon icon in FlyingResourceIcons) icon.UpdateAnimation(Time.deltaTime);
        foreach (FlyingResourceIcon icon in FlyingResourceIcons.Where(s => s.IsDone)) GameObject.Destroy(icon.gameObject);
        FlyingResourceIcons = FlyingResourceIcons.Where(s => !s.IsDone).ToList();

        // New spawned icons
        if (Timer - LastStateEventTime >= RESOURCE_ICON_INTERVAL)
        {
            // Spawn a new resource icon on each object
            foreach (GardenSector sector in Sectors)
            {
                foreach (Object scatteredObj in sector.Objects)
                {
                    // Skip object if still in shed
                    if (scatteredObj.IsInShed) continue;

                    // Get production of object
                    Dictionary<ResourceDef, ResourceProduction> objProduction = Game.Instance.GetTileProduction(scatteredObj.Tile);
                    if (objProduction.Count == 0) continue;

                    // Find resource to scatter (deterministic, no per-iteration allocations of Keys.ToList)
                    ResourceDef res = null;

                    // Build a stable, sorted list of (ResourceDef, Production)
                    var pairs = new List<KeyValuePair<ResourceDef, ResourceProduction>>(objProduction.Count);
                    foreach (var kv in objProduction) pairs.Add(kv);
                    pairs.Sort((a, b) => string.CompareOrdinal(a.Key.DefName, b.Key.DefName));

                    // Walk the cumulative counts to pick the k-th unit (k = StateIndex)
                    int k = StateIndex;
                    for (int i = 0; i < pairs.Count; i++)
                    {
                        int count = Mathf.Abs(pairs[i].Value.GetValue());
                        if (k < count)
                        {
                            res = pairs[i].Key;
                            break;
                        }
                        k -= count;
                    }
                    Debug.Log($"Resource Distribution: Index for {scatteredObj.LabelCapWord} is resource {res}");
                    if (res == null) continue;

                    // Init resource icon
                    bool isNegative = objProduction[res].GetValue() < 0;
                    ResourceDef resToFly = res;
                    MapTile sourceTile = scatteredObj.Tile;
                    GameObject resObject = new GameObject($"Flying {res.LabelCapWord}");
                    resObject.transform.SetParent(GameUI.Instance.FlyingResourceIconsContainer.transform);
                    FlyingResourceIcon icon = resObject.AddComponent<FlyingResourceIcon>();
                    icon.Init(resToFly, sourceTile, isNegative);
                    FlyingResourceIcons.Add(icon);
                }
                
            }

            LastStateEventTime = Timer;
            StateIndex++;
        }

        // Check if everything done
        if (StateIndex >= MaxResourcesProduced && FlyingResourceIcons.Count == 0)
        {
            SwitchStateTo(HarvestAnimationState.ExistingModifierTicker);
        }
    }

    private static void UpdateExistingModifierTicker()
    {
        // Advance animations
        for (int i = 0; i < FizzlingModifiers.Count; i++)
        {
            FizzlingModifiers[i].UpdateAnimation(Time.deltaTime);
        }

        // Destroy finished, compact list
        for (int i = FizzlingModifiers.Count - 1; i >= 0; i--)
        {
            if (FizzlingModifiers[i].IsDone)
            {
                GameObject.Destroy(FizzlingModifiers[i].gameObject);
                FizzlingModifiers.RemoveAt(i);
            }
        }

        // Spawn new fizzle FX in (in waves, iterated through modifier index on each tile).
        if (Timer - LastStateEventTime >= MODIFIER_INTERVAL)
        {
            foreach (MapTile tile in Game.Instance.Map.OwnedTiles)
            {
                // Get total amount of modifiers
                int numTotalModifiers = tile.GetTotalAmountOfModifiers();

                // Get modifier we want to decrease duration
                if (StateIndex >= numTotalModifiers) continue;
                Modifier modifierToTick = null;
                if (StateIndex < tile.Modifiers.Count) modifierToTick = tile.Modifiers[StateIndex];
                else modifierToTick = tile.Object.Modifiers[StateIndex - tile.Modifiers.Count];

                // Decrease duration
                modifierToTick.DecreaseDuration();

                // If modifier is expired, spawn a fizzle out animation
                if (modifierToTick.IsExpired)
                {
                    // Spawn a GO at the tile center
                    Vector3 worldPos = new Vector3(tile.Coordinates.x + 0.5f, tile.Coordinates.y + 0.5f, 0f);
                    GameObject go = new GameObject($"Fizzling {modifierToTick.LabelCapWord}");
                    var fizz = go.AddComponent<FizzlingModifier>();
                    fizz.Init(modifierToTick, worldPos, MODIFIER_FIZZLE_DURATION);

                    FizzlingModifiers.Add(fizz);

                    Game.Instance.DrawFullMap(); // Redraw map so modifier isn't drawn anymore on tilemap
                }
            }

            LastStateEventTime = Timer;
            StateIndex++;
        }

        // Completion gate: when we've iterated past the longest sector list AND nothing is active.
        int maxModifiers = Game.Instance.Map.OwnedTiles.Max(t => t.GetTotalAmountOfModifiers());

        if (StateIndex >= maxModifiers && FizzlingModifiers.Count == 0)
        {
            SwitchStateTo(HarvestAnimationState.NewModifierApplication);
        }
    }

    private static void UpdateNewModifierApplication()
    {
        // Todo: First stage all modifiers that will be applied, grouped by source tile (Dictionary<MapTile, Modifier>)
        // Do that before switching to this state

        // In this update, iterate through these groups, and start sending modifier sprites 1 by 1
        // on arrive, apply the modifier and redraw map

        SwitchStateTo(HarvestAnimationState.ObjectsReturning);
        Game.Instance.DrawFullMap(); // To open shed doors
    }

    private static void UpdateObjectsReturning()
    {
        // Flying objects
        foreach (FlyingObjectSprite so in ReturningObjects) so.UpdateAnimation(Time.deltaTime);
        foreach (FlyingObjectSprite so in ReturningObjects.Where(s => s.IsDone)) GameObject.Destroy(so.gameObject);
        ReturningObjects = ReturningObjects.Where(s => !s.IsDone).ToList();

        // New spawned objects
        if (Timer - LastStateEventTime >= OBJECT_INTERVAL)
        {
            // Spawn an object in each sector
            foreach (GardenSector sector in Sectors)
            {
                if (StateIndex >= sector.CurrentScatter.Count) continue;

                // Init scatter object
                Object objToScatter = sector.CurrentScatter.Keys.ToList()[StateIndex];
                MapTile sourceTile = objToScatter.Tile;
                MapTile targetTile = sector.ShedTile;
                GameObject scatteringObjGo = new GameObject($"Flying {objToScatter.LabelCapWord}");
                FlyingObjectSprite returningObject = scatteringObjGo.AddComponent<FlyingObjectSprite>();
                returningObject.Init(objToScatter, sourceTile, targetTile, onArriveCallback: OnObjectReturned);
                ReturningObjects.Add(returningObject);

                // Remove object from map
                Game.Instance.ClearTile(sourceTile);
                Game.Instance.DrawFullMap();
            }

            LastStateEventTime = Timer;
            StateIndex++;
        }

        // Check if everything done
        if (StateIndex >= Sectors.Max(s => s.CurrentScatter.Count) && ReturningObjects.Count == 0)
        {
            SwitchStateTo(HarvestAnimationState.FinalWait);
            Game.Instance.DrawFullMap(); // To close shed doors
        }
    }

    private static void OnObjectReturned(Object obj, MapTile tile)
    {
        Game.Instance.OnObjectReturnedDuringHarvest(obj);
    }

    private static void UpdateFinalWait()
    {
        if (Timer >= END_DELAY)
        {
            SwitchStateTo(HarvestAnimationState.Finished);
            Callback?.Invoke();
        }
    }


    public enum HarvestAnimationState
    {
        InitialWait,
        ResourceDistribution,
        ExistingModifierTicker,
        NewModifierApplication,
        ObjectsReturning,
        FinalWait,
        Finished
    }
}
