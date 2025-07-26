using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The outcome / actual effect of an ObjectEffect defining what happens to a target tile when the effect criteria of an effect is fulfilled.
/// </summary>
public class EffectOutcome
{
    /// <summary>
    /// The amount of production that gets modified on all resources an object produces natively.
    /// </summary>
    public int NativeProductionModifier { get; init; } = 0;

    /// <summary>
    /// The amount of production that gets modified on specific resources on a tile, disregarding what resources the object produces natively.
    /// </summary>
    public Dictionary<ResourceDef, int> ResourceProductionModifier { get; init; } = new();

    /// <summary>
    /// The modifier that gets applied to the object on the target tile.
    /// </summary>
    public ModifierDef AppliedModifier { get; init; }

    /// <summary>
    /// Checks if this criteria is valid the way it is defined.
    /// </summary>
    public bool Validate(out string invalidReason)
    {
        invalidReason = "";

        if(NativeProductionModifier == 0 && ResourceProductionModifier.Count == 0 && AppliedModifier == null)
        {
            invalidReason = "There is no outcome effect defined.";
            return false;
        }

        return true;
    }

    public void ApplyProductionModifiersTo(MapTile tile, string source, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        // Apply bonus to native resources
        if (NativeProductionModifier != 0)
        {
            foreach (ResourceDef resource in tile.Object.NativeResources)
            {
                ProductionModifier modifier = new ProductionModifier(source, ProductionModifierType.Additive, NativeProductionModifier);
                tileProductions[tile][resource].AddModifier(modifier);
            }
        }

        // Apply bonus to specific resources
        foreach (var kvp in ResourceProductionModifier)
        {
            ResourceDef resource = kvp.Key;
            int productionBonus = kvp.Value;

            ProductionModifier modifier = new ProductionModifier(source, ProductionModifierType.Additive, productionBonus);
            tileProductions[tile][resource].AddModifier(modifier);
        }
    }

    public void ApplyObjectModifiersTo(Object obj)
    {
        if (obj == null) throw new System.Exception("Object must not be null");

        if (AppliedModifier != null)
        {
            obj.ApplyModifier(AppliedModifier);
        }
    }

    /// <summary>
    /// Returns the generalized "+1 to all native production" "-2 to food and fiber production" string.
    /// </summary>
    public string GetAsReadableString()
    {
        string desc = "";
        if (NativeProductionModifier != 0)
        {
            string sign = NativeProductionModifier > 0 ? "+" : "";
            desc += $"<nobr>{sign}{NativeProductionModifier} native production, </nobr>";
        }
        foreach (var kvp in ResourceProductionModifier)
        {
            string sign = kvp.Value > 0 ? "+" : "";
            desc += $"<nobr>{sign}{kvp.Value} {kvp.Key.GetTooltipLink()} production, </nobr>";
        }
        if (AppliedModifier != null)
        {
            desc += $"<nobr>{AppliedModifier.GetTooltipLink()}, </nobr>";
        }

        // Remove trailing ", </nobr>" if present
        const string trailing = ", </nobr>";
        if (desc.EndsWith(trailing))
        {
            desc = desc.Substring(0, desc.Length - trailing.Length) + "</nobr>";
        }

        return desc;
    }

    public EffectOutcome GetCopy()
    {
        return new EffectOutcome()
        {
            NativeProductionModifier = this.NativeProductionModifier,
            ResourceProductionModifier = new Dictionary<ResourceDef, int>(this.ResourceProductionModifier),
            AppliedModifier = this.AppliedModifier,
        };
    }
}
