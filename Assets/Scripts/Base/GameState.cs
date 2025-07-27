using UnityEngine;

public enum GameState
{
    Uninitialized,
    Morning,
    Noon,
    Afternoon,
    Evening,
    Night_OrderSelection, // only at end of week
    Night_ObjectDraft,
    GameOver,
}
