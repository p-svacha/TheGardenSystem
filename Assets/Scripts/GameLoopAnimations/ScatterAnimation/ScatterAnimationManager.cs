using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that handles the scatter animation during noon.
/// </summary>
public static class ScatterAnimationManager
{
    private const float START_DELAY = 0.7f;
    private const float OBJECT_INTERVAL = 0.35f;
    public const float OBJECT_FYLING_SPEED = 2.5f;
    private const float END_DELAY = 0.7f;

    private static AnimationState State;
    private static float Timer;
    private static float LastObjectTime;
    private static int ObjectIndex;
    private static List<GardenSector> Sectors;
    private static List<FlyingObjectSprite> ScatteringObjects;

    private static System.Action Callback;

    public static void StartAnimation(System.Action callback)
    {
        Callback = callback;

        // Reset values
        Timer = 0f;
        LastObjectTime = 0f;
        Sectors = new List<GardenSector>(Game.Instance.Sectors);
        ObjectIndex = 0;
        ScatteringObjects = new List<FlyingObjectSprite>();
        State = AnimationState.PreScatter;

        // Open all shed doors
        Game.Instance.DrawFullMap();
    }

    public static void Update()
    {
        Timer += Time.deltaTime;

        switch (State)
        {
            case AnimationState.PreScatter:
                UpdatePreScatter();
                break;

            case AnimationState.OjectsScattering:
                UpdateScatter();
                break;

            case AnimationState.PostScatter:
                UpdatePostScatter();
                break;
        }
    }

    private static void SwitchStateTo(AnimationState state)
    {
        State = state;
        Timer = 0f;
    }

    private static void UpdatePreScatter()
    {
        if (Timer >= START_DELAY)
        {
            SwitchStateTo(AnimationState.OjectsScattering);
        }
    }

    private static void UpdateScatter()
    {
        // Flying objects
        foreach (FlyingObjectSprite so in ScatteringObjects) so.UpdateAnimation(Time.deltaTime);
        foreach (FlyingObjectSprite so in ScatteringObjects.Where(s => s.IsDone)) GameObject.Destroy(so.gameObject);
        ScatteringObjects = ScatteringObjects.Where(s => !s.IsDone).ToList();

        // New spawned objects
        if (Timer - LastObjectTime >= OBJECT_INTERVAL)
        {
            // Spawn an object in each sector
            foreach(GardenSector sector in Sectors)
            {
                if (ObjectIndex >= sector.CurrentScatter.Count) continue;

                // Init scatter object
                Object objToScatter = sector.CurrentScatter.Keys.ToList()[ObjectIndex];
                MapTile sourceTile = sector.ShedTile;
                MapTile targetTile = sector.CurrentScatter[objToScatter];
                GameObject scatteringObjGo = new GameObject($"Flying {objToScatter.LabelCapWord}");
                FlyingObjectSprite scatteringObject = scatteringObjGo.AddComponent<FlyingObjectSprite>();
                scatteringObject.Init(objToScatter, sourceTile, targetTile, onArriveCallback: OnObjectLanded);
                ScatteringObjects.Add(scatteringObject);
                objToScatter.IsInShed = false;

                // Remove object from shed window if open
                if (UI_ShedWindow.Instance.gameObject.activeSelf && UI_ShedWindow.Instance.DisplayedSector == sector)
                {
                    UI_ShedWindow.Instance.Show(sector);
                }
            }
            
            LastObjectTime = Timer;
            ObjectIndex++;
        }

        // Check if everything done
        if (ObjectIndex >= Sectors.Max(s => s.CurrentScatter.Count) && ScatteringObjects.Count == 0)
        {
            SwitchStateTo(AnimationState.PostScatter);
        }
    }

    private static void UpdatePostScatter()
    {
        if (Timer >= END_DELAY)
        {
            SwitchStateTo(AnimationState.Finished);
            Callback?.Invoke();
        }
    }

    private static void OnObjectLanded(Object obj, MapTile tile)
    {
        Game.Instance.OnObjectArrivedDuringScatter(obj, tile);
    }

    private enum AnimationState
    {
        PreScatter,
        OjectsScattering,
        PostScatter,
        Finished
    }
}
