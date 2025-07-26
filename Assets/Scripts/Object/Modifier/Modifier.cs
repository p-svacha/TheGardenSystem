using UnityEngine;

/// <summary>
/// A modifier that can modularly get applied to any object or terrain. It holds an effect that - if the criteria is fulfilled - gets applied to it.
/// </summary>
public class Modifier
{
    public ModifierDef Def { get; private set; }
}
