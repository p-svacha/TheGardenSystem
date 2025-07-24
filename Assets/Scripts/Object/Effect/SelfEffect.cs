using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to the object on the source tile based on its attributes.
/// </summary>
public class SelfEffect : ObjectEffect
{
    /// <summary>
    /// If not empty, the object on the source tile has to have any of these tags for the effect to be applied to it.
    /// </summary>
    public List<ObjectTagDef> EffectCriteria_TagsAny { get; init; } = new();

    /// <summary>
    /// If not empty, the object on the source tile has to have all of these tags for the effect to be applied to it.
    /// </summary>
    public List<ObjectTagDef> EffectCriteria_TagsAll { get; init; } = new();

    /// <summary>
    /// If not empty, the object on the source tile has to produce one of these resources for the effect to be applied to it.
    /// </summary>
    public List<ResourceDef> EffectCriteria_NativeProduction_Any { get; init; } = new();

    /// <summary>
    /// The amount of production that gets to added all resources that the object on the source tile produces natively (according to Object.GetBaseResourceProduction).
    /// <br/>The bonus gets applied if the object on the source tile fulfills the EffectCriteria.
    /// </summary>
    public int NativeProductionModifier { get; init; } = 0;

    /// <summary>
    /// The amount of production that gets to added to specific resources of the object on the source tile, disregarding what resources the it produces natively.
    /// <br/>The bonus gets applied if the object on the source tile fulfills the EffectCriteria.
    /// </summary>
    public Dictionary<ResourceDef, int> ResourceProductionModifier { get; init; } = new();

    public override bool Validate(out string invalidReason)
    {
        if (EffectCriteria_TagsAny.Any(t => t == null))
        {
            invalidReason = "EffectCriteria_TagsAny contains a tag that is null.";
            return false;
        }
        if (EffectCriteria_TagsAll.Any(t => t == null))
        {
            invalidReason = "EffectCriteria_TagsAll contains a tag that is null.";
            return false;
        }
        if (EffectCriteria_NativeProduction_Any.Any(t => t == null))
        {
            invalidReason = "EffectCriteria_NativeProduction_Any contains a resource that is null.";
            return false;
        }

        if (EffectCriteria_TagsAny.Count == 0 && EffectCriteria_TagsAll.Count == 0 && EffectCriteria_NativeProduction_Any.Count == 0)
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
        bool doApplyEffect = true;

        if (!sourceTile.HasObject) doApplyEffect = false;
        else if (EffectCriteria_TagsAny.Count > 0 && !sourceTile.Object.HasAnyOfTags(EffectCriteria_TagsAny)) doApplyEffect = false;
        else if (EffectCriteria_TagsAll.Count > 0 && !sourceTile.Object.HasAllTags(EffectCriteria_TagsAll)) doApplyEffect = false;
        else if (EffectCriteria_NativeProduction_Any.Count > 0 && !sourceTile.Object.ProducesAnyOfResourcesNatively(EffectCriteria_NativeProduction_Any)) doApplyEffect = false;
        if (!doApplyEffect) return; // Abort if effect shouldn't be applied

        Object sourceObject = sourceTile.Object;

        // Apply bonus to native resources
        if (NativeProductionModifier != 0)
        {
            foreach (ResourceDef resource in sourceObject.NativeResources)
            {
                ProductionModifier modifier = new ProductionModifier(source: sourceTile.Terrain.Def, ProductionModifierType.Additive, NativeProductionModifier);
                tileProductions[sourceTile][resource].AddModifier(modifier);
            }
        }

        // Apply bonus to specific resources
        foreach (var kvp in ResourceProductionModifier)
        {
            ResourceDef resource = kvp.Key;
            int productionBonus = kvp.Value;

            ProductionModifier modifier = new ProductionModifier(source: sourceTile.Terrain.Def, ProductionModifierType.Additive, productionBonus);
            tileProductions[sourceTile][resource].AddModifier(modifier);
        }
    }

    public override string GetDescription()
    {
        string bonusText = GetBonusPartOfDescription(NativeProductionModifier, ResourceProductionModifier);
        string criteriaText = GetCriteriaPartDescription(EffectCriteria_TagsAny, EffectCriteria_TagsAll, EffectCriteria_NativeProduction_Any, plural: true);
        return $"{bonusText} to {criteriaText}.";
    }
}
