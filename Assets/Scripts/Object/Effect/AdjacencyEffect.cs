using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to adjacent objects of a source tile. 
/// </summary>
public class AdjacencyEffect : ObjectEffect
{
    public int Radius { get; set; } = 1;

    public override void ApplyProductionModifiers(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles(Radius))
        {
            if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(adjacentTile)) continue;

            string source = $"Adjacent {EffectSource.GetTooltipTitle()}";
            EffectOutcome.ApplyProductionModifiersTo(adjacentTile, source, tileProductions);
        }
    }

    public override void ApplyObjectAndTileModifiers(MapTile sourceTile)
    {
        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles(Radius))
        {
            if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(adjacentTile)) continue;
            EffectOutcome.ApplyModifiersTo(adjacentTile);
        }
    }

    public override string GetDescription()
    {
        string bonusText = EffectOutcome.GetAsReadableString();
        string criteriaText = EffectCriteria.GetAsReadableString(plural: true);
        string radiusText = Radius == 1 ? "" : $" in a {Radius} tile radius";
        return $"{bonusText} to adjacent {criteriaText}{radiusText}.";
    }

    public override ObjectEffect GetCopy()
    {
        AdjacencyEffect copy = new AdjacencyEffect();
        copy.SetValuesFrom(this);
        copy.Radius = this.Radius;
        return copy;
    }
}
