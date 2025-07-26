using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to the object on the source tile based on its attributes.
/// </summary>
public class SelfEffect : ObjectEffect
{
    public override void ApplyEffectTo(MapTile tile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        if (!EffectCriteria.IsFulfilledOn(tile)) return;
        EffectOutcome.ApplyTo(tile, EffectSource, tileProductions);
    }

    public override string GetDescription()
    {
        string bonusText = EffectOutcome.GetAsReadableString();
        string criteriaText = EffectCriteria.GetAsReadableString(plural: true);
        return $"{bonusText} to {criteriaText}.";
    }

    public override ObjectEffect GetCopy()
    {
        SelfEffect copy = new SelfEffect();
        copy.SetValuesFrom(this);
        return copy;
    }
}
