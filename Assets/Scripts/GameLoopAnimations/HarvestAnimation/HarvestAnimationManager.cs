using System.Collections.Generic;
using UnityEngine;

public static class HarvestAnimationManager
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
    private static List<ScatteringObject> ReturningObjects;

    private static System.Action Callback;

    public static void StartAnimation(System.Action callback)
    {
        Callback = callback;

        // Reset values
        Timer = 0f;
        LastObjectTime = 0f;
        Sectors = new List<GardenSector>(Game.Instance.Sectors);
        ObjectIndex = 0;
        ReturningObjects = new List<ScatteringObject>();
        State = AnimationState.InitialWait;

        // Open all shed doors
        Game.Instance.DrawFullMap();
    }

    public static void Update()
    {
        Timer += Time.deltaTime;

        switch (State)
        {
            case AnimationState.InitialWait:
                UpdateInitialWait();
                break;

            case AnimationState.ModifierApplication:
                UpdateModifierApplication();
                break;

            case AnimationState.ObjectsReturning:
                UpdateObjectsReturning();
                break;

            case AnimationState.FinalWait:
                UpdateFinalWait();
                break;
        }
    }

    private static void SwitchStateTo(AnimationState state)
    {
        State = state;
        Timer = 0f;
    }

    private static void UpdateInitialWait()
    {
        if (Timer >= START_DELAY)
        {
            SwitchStateTo(AnimationState.FinalWait);
        }
    }

    private static void UpdateModifierApplication()
    {
    }

    private static void UpdateObjectsReturning()
    {
    }

    private static void UpdateFinalWait()
    {
        if (Timer >= END_DELAY)
        {
            SwitchStateTo(AnimationState.Finished);
            Callback?.Invoke();
        }
    }

    private enum AnimationState
    {
        InitialWait,
        ModifierApplication,
        ObjectsReturning,
        FinalWait,
        Finished
    }
}
