using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to the object on the source tile based on its attributes.
/// </summary>
public class SelfEffect : ObjectEffect
{
    public override void ApplyProductionModifiers(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(sourceTile)) return;
        EffectOutcome.ApplyProductionModifiersTo(sourceTile, EffectSource, tileProductions);
    }

    public override void ApplyObjectModifiers(MapTile sourceTile)
    {
        if (!sourceTile.HasObject) return;
        if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(sourceTile)) return;
        EffectOutcome.ApplyObjectModifiersTo(sourceTile.Object);
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
