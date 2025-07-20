using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An ObjectEffect defines a specific interaction of an object.
/// </summary>
public abstract class ObjectEffect
{
    /// <summary>
    /// Returns if this is a valid ObjectEffect, including a reason of why if it is not.
    /// </summary>
    public abstract bool Validate(out string invalidReason);

    /// <summary>
    /// Applies all resource production modifiers that originate from this effect.
    /// </summary>
    public abstract void ApplyEffect(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions);
}
