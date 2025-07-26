using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to adjacent objects of a source tile. 
/// </summary>
public class AdjacencyEffect : ObjectEffect
{
    public override void ApplyProductionModifiers(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles())
        {
            if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(adjacentTile)) continue;

            string source = $"Adjacent {EffectSource.GetTooltipTitle()}";
            EffectOutcome.ApplyProductionModifiersTo(adjacentTile, source, tileProductions);
        }
    }

    public override void ApplyObjectModifiers(MapTile sourceTile)
    {
        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles())
        {
            if (!adjacentTile.HasObject) continue;
            if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(adjacentTile)) continue;
            EffectOutcome.ApplyObjectModifiersTo(adjacentTile.Object);
        }
    }

    public override string GetDescription()
    {
        string bonusText = EffectOutcome.GetAsReadableString();
        string criteriaText = EffectCriteria.GetAsReadableString(plural: true);
        return $"{bonusText} to adjacent {criteriaText}.";
    }

    public override ObjectEffect GetCopy()
    {
        AdjacencyEffect copy = new AdjacencyEffect();
        copy.SetValuesFrom(this);
        return copy;
    }
}
