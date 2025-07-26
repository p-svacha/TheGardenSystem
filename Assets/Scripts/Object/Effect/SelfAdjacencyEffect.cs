using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to the object on the source tile based on all objects around it.
/// </summary>
public class SelfAdjacencyEffect : ObjectEffect
{
    public override void ApplyProductionModifiers(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles())
        {
            if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(adjacentTile)) continue;
            EffectOutcome.ApplyProductionModifiersTo(sourceTile, EffectSource, tileProductions);
        }
    }
    public override void ApplyObjectModifiers(MapTile sourceTile)
    {
        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles())
        {
            if (!adjacentTile.HasObject) continue;
            if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(adjacentTile)) continue;
            EffectOutcome.ApplyObjectModifiersTo(sourceTile.Object);
        }
    }

    public override string GetDescription()
    {
        string bonusText = EffectOutcome.GetAsReadableString();
        string criteriaText = EffectCriteria.GetAsReadableString();
        return $"{bonusText} for each adjacent {criteriaText}.";
    }

    public override ObjectEffect GetCopy()
    {
        SelfAdjacencyEffect copy = new SelfAdjacencyEffect();
        copy.SetValuesFrom(this);
        return copy;
    }
}
