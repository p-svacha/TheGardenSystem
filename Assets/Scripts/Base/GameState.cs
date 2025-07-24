using UnityEngine;

public enum GameState
{
    Uninitialized,
    BeforeScatter,
    ScatterManipulation,
    ConfirmedScatter,
    OrderSelection, // only at end of week
    ObjectDraft,
}
