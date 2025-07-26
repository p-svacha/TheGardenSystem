using UnityEngine;

/// <summary>
/// A modifier that can modularly get applied to any object or terrain. It holds an effect that - if the criteria is fulfilled - gets applied to it.
/// </summary>
public class Modifier
{
    public ModifierDef Def { get; private set; }

    public int StartDuration { get; private set; }
    public int RemainingDuration { get; private set; }

    public Modifier(ModifierDef def, int duration = -1)
    {
        Def = def;
        SetDuration(duration);
    }

    public void DecreaseDuration(int amount = 1)
    {
        if(RemainingDuration != -1)
        {
            RemainingDuration -= amount;
            if (RemainingDuration < 0) RemainingDuration = 0;
        }
    }

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

    public ObjectEffect Effect => Def.Effect;
}
