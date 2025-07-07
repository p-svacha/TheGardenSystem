using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An ObjectEffect defines a specific interaction of an object.
/// </summary>
public abstract class ObjectEffect
{
    /// <summary>
    /// Applies all resource production modifiers that originate from this effect.
    /// </summary>
    public abstract void ApplyEffect(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions);
}
