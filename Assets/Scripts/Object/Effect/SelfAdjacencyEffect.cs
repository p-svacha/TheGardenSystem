using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to a source object itself based on all objects around it.
/// </summary>
public class SelfAdjacencyEffect : ObjectEffect
{
    /// <summary>
    /// If not empty, the effect is applied to the source object for each adjacent object with a tag included in this list.
    /// </summary>
    public List<ObjectTagDef> EffectCriteria_TagsAny { get; init; } = new();

    /// <summary>
    /// If not empty, the effect is applied to the source object for each adjacent object that has all tags included in this list.
    /// </summary>
    public List<ObjectTagDef> EffectCriteria_TagsAll { get; init; } = new();

    /// <summary>
    /// The amount of production that gets to added all resources that the source object produces natively (according to Object.GetBaseResourceProduction).
    /// <br/>The bonus gets applied to the source object for each adjacent object that fulfills the EffectCriteria.
    /// </summary>
    public int GeneralProductionBonus { get; init; } = 0;

    /// <summary>
    /// The amount of production that gets to added to specific resources of the source object, disregarding what resources the it produces natively.
    /// <br/>The bonus gets applied to the source object for each adjacent object that fulfills the EffectCriteria.
    /// </summary>
    public Dictionary<ResourceDef, int> ResourceProductionBonus { get; init; } = new();

    public override bool Validate(out string invalidReason)
    {
        if (EffectCriteria_TagsAny.Count == 0 && EffectCriteria_TagsAll.Count == 0)
        {
            invalidReason = "There is no criteria defined for when this effect should be triggered.";
            return false;
        }
        if (EffectCriteria_TagsAny.Count > 0 && EffectCriteria_TagsAll.Count > 0)
        {
            invalidReason = "There is conflicting criteria defined for when this effect should be triggered.";
            return false;
        }

        invalidReason = "";
        return true;
    }

    public override void ApplyEffect(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        if (!sourceTile.HasObject) return; // No object on this tile to apply bonuses to
        Object sourceObject = sourceTile.Object;

        foreach (MapTile adjacentTile in sourceTile.GetAdjacentTiles())
        {
            // Check if effect can be applied
            bool doApplyEffect = true;

            if (!adjacentTile.HasObject) doApplyEffect = false;
            else if (EffectCriteria_TagsAny.Count > 0 && (!adjacentTile.Object.HasAnyOfTags(EffectCriteria_TagsAny))) doApplyEffect = false;
            else if (EffectCriteria_TagsAll.Count > 0 && (!adjacentTile.Object.HasAllTags(EffectCriteria_TagsAll))) doApplyEffect = false;

            if (!doApplyEffect) continue;

            // Apply bonus to native resources
            if (GeneralProductionBonus > 0)
            {
                foreach (ResourceDef resource in sourceObject.NativeResources)
                {
                    ProductionModifier modifier = new ProductionModifier(source: adjacentTile.Object, ProductionModifierType.Additive, GeneralProductionBonus);
                    tileProductions[sourceTile][resource].AddModifier(modifier);
                }
            }

            // Apply bonus to specific resources
            foreach (var kvp in ResourceProductionBonus)
            {
                ResourceDef resource = kvp.Key;
                int productionBonus = kvp.Value;

                ProductionModifier modifier = new ProductionModifier(source: adjacentTile.Object, ProductionModifierType.Additive, productionBonus);
                tileProductions[sourceTile][resource].AddModifier(modifier);
            }
        }
    }
}
