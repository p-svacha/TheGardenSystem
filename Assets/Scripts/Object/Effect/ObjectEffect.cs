using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An ObjectEffect defines a specific effect originating from a source tile, that affects the resource production of tiles or objects.
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

    /// <summary>
    /// Returns what this effect does as a human-readable string including TMPro tooltip links.
    /// </summary>
    public abstract string GetDescription();
}
