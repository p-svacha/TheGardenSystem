using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An ObjectEffect defines a specific effect applied to an object originating from a source (which can be the object itself, another object, or terrain).
/// Eact ObjectEffect consists of a criteria and an outcome. The criteria defines the conditions of WHEN the effect is applied, and the outcome defines WHAT is applied.
/// </summary>
public abstract class ObjectEffect
{
    /// <summary>
    /// The object that is the source of this effect, as a tooltip target.
    /// Should be set in ResolveReferences of the Def holding the effect.
    /// </summary>
    public ITooltipTarget EffectSource { get; set; }

    /// <summary>
    /// The criteria of when this effect should be triggered. May be null if the effect should always be applied.
    /// </summary>
    public EffectCriteria EffectCriteria { get; set; }

    /// <summary>
    /// The outcome defining what happens when this effect is applied.
    /// </summary>
    public EffectOutcome EffectOutcome { get; set; }

    /// <summary>
    /// Applies all resource production modifiers that originate from this effect, if the criteria is fulfilled.
    /// </summary>
    public abstract void ApplyProductionModifiers(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions);

    /// <summary>
    /// Applies all object modifiers that originate from this effect, if the criteria is fulfilled.
    /// </summary>
    public abstract void ApplyObjectAndTileModifiers(MapTile sourceTile);

    /// <summary>
    /// Returns what this effect does as a human-readable string including TMPro tooltip links.
    /// </summary>
    public abstract string GetDescription();

    /// <summary>
    /// Returns if this is a valid ObjectEffect, including a reason of why if it is not.
    /// </summary>
    public bool Validate(out string invalidReason)
    {
        if(EffectCriteria != null)
        {
            if (!EffectCriteria.Validate(out invalidReason)) return false;
        }

        if (EffectOutcome == null)
        {
            invalidReason = "No effect outcome has been defined.";
            return false;
        }
        if (!EffectOutcome.Validate(out invalidReason)) return false;

        return true;
    }

    public ObjectEffect() { }

    protected void SetValuesFrom(ObjectEffect orig)
    {
        if(orig.EffectCriteria != null) EffectCriteria = orig.EffectCriteria.GetCopy();
        EffectOutcome = orig.EffectOutcome.GetCopy();
    }

    public abstract ObjectEffect GetCopy();
}
