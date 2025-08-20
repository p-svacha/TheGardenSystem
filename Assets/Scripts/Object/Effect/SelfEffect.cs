using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that an object applies to itself if the criteria is fulfilled.
/// </summary>
public class SelfEffect : ObjectEffect
{
    public override void ApplyProductionModifiers(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(sourceTile)) return;

        string source = "";
        if (EffectSource is ModifierDef modifier) source += modifier.GetTooltipLink();
        if (EffectCriteria != null)
        {
            string criteriaDesc = EffectCriteria.GetAsReadableString(includeObjectLiteral: false).CapitalizeFirst();
            if (source == "") source = criteriaDesc;
            else source += $" ({criteriaDesc})";
        }
        EffectOutcome.ApplyProductionModifiersTo(sourceTile, source, tileProductions);
    }

    public override void ApplyObjectAndTileModifiers(MapTile sourceTile)
    {
        if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(sourceTile)) return;
        EffectOutcome.ApplyModifiersTo(sourceTile);
    }

    public override string GetDescription()
    {
        string bonusText = EffectOutcome.GetAsReadableString();
        string criteriaText = EffectCriteria == null ? "" : $" if {EffectCriteria.GetAsReadableString(includeObjectLiteral: false)}";
        return $"{bonusText}{criteriaText}.";
    }

    public override ObjectEffect GetCopy()
    {
        SelfEffect copy = new SelfEffect();
        copy.SetValuesFrom(this);
        return copy;
    }
}
