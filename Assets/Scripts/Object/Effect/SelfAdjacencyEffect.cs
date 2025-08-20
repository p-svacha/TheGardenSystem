using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to the object on the source tile based on all objects around it.
/// </summary>
public class SelfAdjacencyEffect : ObjectEffect
{
    public int Radius { get; init; } = 1;

    public override void ApplyProductionModifiers(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles(Radius))
        {
            if (EffectCriteria != null && !EffectCriteria.IsFulfilledOn(adjacentTile)) continue;

            string source = $"Adjacent {EffectCriteria.GetAsReadableString()}";
            EffectOutcome.ApplyProductionModifiersTo(sourceTile, source, tileProductions);
        }
    }

    public override List<Modifier> GetObjectModifiersToApply(MapTile sourceTile)
    {
        List<Modifier> modifiers = new List<Modifier>();

        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles(Radius))
        {
            Modifier modifier = TryCreateObjectModifierFor(sourceTile, adjacentTile, sourceTile);
            if (modifier != null) modifiers.Add(modifier);
        }

        return modifiers;
    }

    public override List<Modifier> GetTileModifiersToApply(MapTile sourceTile)
    {
        List<Modifier> modifiers = new List<Modifier>();

        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles(Radius))
        {
            Modifier modifier = TryCreateTileModifierFor(sourceTile, adjacentTile, sourceTile);
            if (modifier != null) modifiers.Add(modifier);
        }

        return modifiers;
    }

    public override string GetDescription()
    {
        string bonusText = EffectOutcome.GetAsReadableString();
        string criteriaText = EffectCriteria.GetAsReadableString();
        string radiusText = Radius == 1 ? "" : $" in a {Radius} tile radius";
        return $"{bonusText} for each adjacent {criteriaText}{radiusText}.";
    }

    public override ObjectEffect GetCopy()
    {
        SelfAdjacencyEffect copy = new SelfAdjacencyEffect();
        copy.SetValuesFrom(this);
        return copy;
    }
}
