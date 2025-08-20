using UnityEngine;

/// <summary>
/// A modifier that can modularly get applied to any object or terrain. It holds an effect that - if the criteria is fulfilled - gets applied to it.
/// </summary>
public class Modifier
{
    public ModifierDef Def { get; private set; }

    public int StartDuration { get; private set; }
    public int RemainingDuration { get; private set; }

    public MapTile Tile { get; private set; }
    public Object Object { get; private set; }

    /// <summary>Create a new object modifier.</summary>
    public Modifier(Object obj, ModifierDef def, int duration = -1)
    {
        Object = obj;
        Init(def, duration);
    }

    /// <summary>Create a new tile modifier.</summary>
    public Modifier(MapTile tile, ModifierDef def, int duration = -1)
    {
        Tile = tile;
        Init(def, duration);
    }

    /// <summary>Base constructor</summary>
    private void Init(ModifierDef def, int duration)
    {
        Def = def;
        SetDuration(duration);
    }

    /// <summary>
    /// Decreases the duration by 1.
    /// If reaching 0, the modifier is removed from its object or tile.
    /// </summary>
    public void DecreaseDuration(int amount = 1)
    {
        if (IsInfinite) return;

        RemainingDuration -= amount;
        if (RemainingDuration < 0)
        {
            RemainingDuration = 0;
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
