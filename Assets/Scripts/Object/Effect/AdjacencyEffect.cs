using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to adjacent objects of a source tile. 
/// </summary>
public class AdjacencyEffect : ObjectEffect
{
    public override void ApplyEffectTo(MapTile tile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        foreach (MapTile adjacentTile in tile.GetAdjacentTiles())
        {
            if (!EffectCriteria.IsFulfilledOn(adjacentTile)) continue;
            EffectOutcome.ApplyTo(adjacentTile, EffectSource, tileProductions);
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
