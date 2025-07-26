using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to the object on the source tile based on all objects around it.
/// </summary>
public class SelfAdjacencyEffect : ObjectEffect
{
    public override void ApplyEffectTo(MapTile tile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        foreach (MapTile adjacentTile in tile.GetAdjacentTiles())
        {
            if (!EffectCriteria.IsFulfilledOn(adjacentTile)) continue;
            EffectOutcome.ApplyTo(tile, EffectSource, tileProductions);
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
