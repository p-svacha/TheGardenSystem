using UnityEngine;

/// <summary>
/// A modifier that can modularly get applied to any object. It holds an effect that - if the criteria is fulfilled - gets applied to it.
/// </summary>
public class ObjectModifier
{
    public ObjectModifierDef Def { get; private set; }
}
