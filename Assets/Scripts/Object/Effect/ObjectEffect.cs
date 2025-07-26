using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An ObjectEffect defines a specific effect originating from a source tile, that affects the resource production of tiles or objects.
/// </summary>
public abstract class ObjectEffect
{
    /// <summary>
    /// The object that is the source of this effect, as a tooltip target.
    /// Should be set in ResolveReferences of the Def holding the effect.
    /// </summary>
    public INestedTooltipTarget EffectSource { get; set; }

    /// <summary>
    /// The criteria of when this effect should be triggered.
    /// </summary>
    public EffectCriteria EffectCriteria { get; set; }

    /// <summary>
    /// The outcome defining what happens when this effect is applied.
    /// </summary>
    public EffectOutcome EffectOutcome { get; set; }

    /// <summary>
    /// Applies all resource production modifiers that originate from this effect.
    /// </summary>
    public abstract void ApplyEffectTo(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions);

    /// <summary>
    /// Returns what this effect does as a human-readable string including TMPro tooltip links.
    /// </summary>
    public abstract string GetDescription();

    /// <summary>
    /// Returns if this is a valid ObjectEffect, including a reason of why if it is not.
    /// </summary>
    public bool Validate(out string invalidReason)
    {
        invalidReason = "";

        if(EffectCriteria == null)
        {
            invalidReason = "No effect criteria has been defined.";
            return false;
        }
        if (EffectOutcome == null)
        {
            invalidReason = "No effect outcome has been defined.";
            return false;
        }

        if (!EffectCriteria.Validate(out invalidReason)) return false;
        if (!EffectOutcome.Validate(out invalidReason)) return false;

        return true;
    }

    public ObjectEffect() { }

    protected void SetValuesFrom(ObjectEffect orig)
    {
        EffectCriteria = orig.EffectCriteria.GetCopy();
        EffectOutcome = orig.EffectOutcome.GetCopy();
    }

    public abstract ObjectEffect GetCopy();
}
