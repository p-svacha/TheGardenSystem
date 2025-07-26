using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A kinf of ObjectEffect that gets applied from a terrain to the object on it.
/// </summary>
public class TerrainEffect : ObjectEffect
{
    public override void ApplyProductionModifiers(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        if (EffectCriteria != null && !sourceTile.HasObject) return; // Terrain effects can only apply to objects
        if (!EffectCriteria.IsFulfilledOn(sourceTile)) return;
        EffectOutcome.ApplyProductionModifiersTo(sourceTile, EffectSource.GetTooltipTitle(), tileProductions);
    }

    public override void ApplyObjectModifiers(MapTile sourceTile)
    {
        if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(sourceTile)) return;
        EffectOutcome.ApplyModifiersTo(sourceTile);
    }

    public override string GetDescription()
    {
        string bonusText = EffectOutcome.GetAsReadableString();
        string criteriaText = EffectCriteria.GetAsReadableString(plural: true);
        return $"{bonusText} to {criteriaText}.";
    }

    public override ObjectEffect GetCopy()
    {
        TerrainEffect copy = new TerrainEffect();
        copy.SetValuesFrom(this);
        return copy;
    }
}
