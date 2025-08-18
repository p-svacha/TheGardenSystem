using UnityEngine;

public enum GameState
{
    Uninitialized,
    Morning,
    Noon,
    Afternoon,
    Evening_HarvestAnimation,
    Evening_PostHarvest,
    Night_OrderSelection, // only at end of week
    Night_ObjectDraft,
    GameOver,
}
