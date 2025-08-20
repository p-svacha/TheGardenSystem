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
    public ModifierDef AppliedObjectModifier { get; init; }

    /// <summary>
    /// How long the modifier applied to the object on the target tile will last. Default is infinite.
    /// </summary>
    public int AppliedObjectModifierDuration { get; init; } = -1;

    /// <summary>
    /// The modifier that gets applied to the target tile.
    /// </summary>
    public ModifierDef AppliedTileModifier { get; init; }

    /// <summary>
    /// How long the modifier applied to the target tile will last. Default is infinite.
    /// </summary>
    public int AppliedTileModifierDuration { get; init; } = -1;

    /// <summary>
    /// Flag if this outcome destroys the object it is applied to.
    /// </summary>
    public bool Destroy { get; init; } = false;

    /// <summary>
    /// Checks if this criteria is valid the way it is defined.
    /// </summary>
    public bool Validate(out string invalidReason)
    {
        invalidReason = "";

        if (NativeProductionModifier == 0 && ResourceProductionModifier.Count == 0 && AppliedObjectModifier == null && AppliedTileModifier == null && !Destroy)
        {
            invalidReason = "There is no outcome effect defined.";
            return false;
        }

        if (Destroy)
        {
            if (NativeProductionModifier != 0 || ResourceProductionModifier.Count != 0 || AppliedObjectModifier != null)
            {
                invalidReason = "Outcome can't change anything on object it is set to destroy.";
                return false;
            }
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
                tileProductions[tile][resource].AddProductionModifier(modifier);
            }
        }

        // Apply bonus to specific resources
        foreach (var kvp in ResourceProductionModifier)
        {
            ResourceDef resource = kvp.Key;
            int productionBonus = kvp.Value;

            ProductionModifier modifier = new ProductionModifier(source, ProductionModifierType.Additive, productionBonus);
            tileProductions[tile][resource].AddProductionModifier(modifier);
        }
    }

    public bool IsApplyingObjectModifier => AppliedObjectModifier != null;
    public bool IsApplyingTileModifier => AppliedTileModifier != null;

    /// <summary>
    /// Applies the destruction command to the given object.
    /// </summary>
    public void ApplyDestructionTo(Object obj)
    {
        if (!Destroy) throw new System.Exception("Can't apply destruction without destroy flag.");

        // todo
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
            desc += $"<nobr>{sign}{kvp.Value} {kvp.Key.GetTooltipLink()}, </nobr>";
        }
        if (AppliedObjectModifier != null)
        {
            string duration = AppliedObjectModifierDuration == -1 ? "" : $" for {AppliedObjectModifierDuration} days";
            desc += $"<nobr>{AppliedObjectModifier.GetTooltipLink()}{duration}, </nobr>";
        }
        if (AppliedTileModifier != null)
        {
            string duration = AppliedTileModifierDuration == -1 ? "" : $" for {AppliedTileModifierDuration} days";
            desc += $"<nobr>Applies {AppliedTileModifier.GetTooltipLink()} to the tile{duration}, </nobr>";
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
            AppliedObjectModifier = this.AppliedObjectModifier,
            AppliedObjectModifierDuration = this.AppliedObjectModifierDuration,
            AppliedTileModifier = this.AppliedTileModifier,
            AppliedTileModifierDuration = this.AppliedTileModifierDuration,
            Destroy = this.Destroy,
        };
    }
}
