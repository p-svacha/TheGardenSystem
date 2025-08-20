using UnityEngine;

/// <summary>
/// A modifier that can modularly get applied to any object or terrain. It holds an effect that - if the criteria is fulfilled - gets applied to it.
/// </summary>
public class Modifier
{
    public ModifierDef Def { get; private set; }

    /// <summary>
    /// The tile where this modifier originated from.
    /// </summary>
    public MapTile SourceTile { get; private set; }

    public int StartDuration { get; private set; }
    public int RemainingDuration { get; private set; }

    /// <summary>
    /// Only set if this is a tile modifier.
    /// </summary>
    public MapTile Tile { get; private set; }

    /// <summary>
    /// Only set if this is an object modifier.
    /// </summary>
    public Object Object { get; private set; }

    public MapTile AttachedTile => Tile != null ? Tile : Object.Tile;

    /// <summary>Create a new object modifier.</summary>
    public Modifier(Object targetObject, ModifierDef def, MapTile sourceTile, int duration = -1)
    {
        Object = targetObject;
        Init(def, sourceTile, duration);
    }

    /// <summary>Create a new tile modifier.</summary>
    public Modifier(MapTile targetTile, ModifierDef def, MapTile sourceTile, int duration = -1)
    {
        Tile = targetTile;
        Init(def, sourceTile, duration);
    }

    /// <summary>Base constructor</summary>
    private void Init(ModifierDef def, MapTile sourceTile, int duration)
    {
        Def = def;
        SourceTile = sourceTile;
        SetDuration(duration);
    }

    /// <summary>
    /// Decreases the duration by 1.
    /// If reaching 0, the modifier is removed from its object or tile.
    /// </summary>
    public void DecreaseDuration(int amount = 1)
    {
        Debug.Log("DecreaseDuration called");
        if (IsInfinite) return;

        RemainingDuration -= amount;
        if (RemainingDuration == 0)
        {
            if (Object != null) Object.RemoveExpiredModifiers();
            if (Tile != null) Tile.RemoveExpiredModifiers();
        }
    }

    public bool IsExpired => !IsInfinite && RemainingDuration == 0;

    public bool IsInfinite => RemainingDuration == -1;
    public void MakeInfinite()
    {
        StartDuration = -1;
        RemainingDuration = -1;
    }
    public void SetDuration(int duration)
    {
        StartDuration = duration;
        RemainingDuration = duration;
    }

    public string LabelCapWord => Def.LabelCapWord;
    public ObjectEffect Effect => Def.Effect;
    public Sprite Sprite => Def.Sprite;
}
