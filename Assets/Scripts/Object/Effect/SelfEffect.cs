using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to the object on the source tile based on its attributes.
/// </summary>
public class SelfEffect : ObjectEffect
{
    /// <summary>
    /// If not empty, the effect is applied to the object on the source tile if it has any tag included in this list.
    /// </summary>
    public List<ObjectTagDef> EffectCriteria_TagsAny { get; init; } = new();

    /// <summary>
    /// If not empty, the effect is applied to the object on the source tile if it has all tags included in this list.
    /// </summary>
    public List<ObjectTagDef> EffectCriteria_TagsAll { get; init; } = new();

    /// <summary>
    /// The amount of production that gets to added all resources that the object on the source tile produces natively (according to Object.GetBaseResourceProduction).
    /// <br/>The bonus gets applied if the object on the source tile fulfills the EffectCriteria.
    /// </summary>
    public int GeneralProductionBonus { get; init; } = 0;

    /// <summary>
    /// The amount of production that gets to added to specific resources of the object on the source tile, disregarding what resources the it produces natively.
    /// <br/>The bonus gets applied if the object on the source tile fulfills the EffectCriteria.
    /// </summary>
    public Dictionary<ResourceDef, int> ResourceProductionBonus { get; init; } = new();

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
        bool doApplyEffect = true;

        if (!sourceTile.HasObject) doApplyEffect = false;
        else if (EffectCriteria_TagsAny.Count > 0 && (!sourceTile.Object.HasAnyOfTags(EffectCriteria_TagsAny))) doApplyEffect = false;
        else if (EffectCriteria_TagsAll.Count > 0 && (!sourceTile.Object.HasAllTags(EffectCriteria_TagsAll))) doApplyEffect = false;
        if (!doApplyEffect) return; // Abort if effect shouldn't be applied

        Object sourceObject = sourceTile.Object;

        // Apply bonus to native resources
        if (GeneralProductionBonus > 0)
        {
            foreach (ResourceDef resource in sourceObject.NativeResources)
            {
                ProductionModifier modifier = new ProductionModifier(source: sourceTile.Terrain.Def, ProductionModifierType.Additive, GeneralProductionBonus);
                tileProductions[sourceTile][resource].AddModifier(modifier);
            }
        }

        // Apply bonus to specific resources
        foreach (var kvp in ResourceProductionBonus)
        {
            ResourceDef resource = kvp.Key;
            int productionBonus = kvp.Value;

            ProductionModifier modifier = new ProductionModifier(source: sourceTile.Terrain.Def, ProductionModifierType.Additive, productionBonus);
            tileProductions[sourceTile][resource].AddModifier(modifier);
        }
    }

    public override string GetDescription()
    {
        // 1) Build the bonus part
        var bonusParts = new List<string>();
        if (GeneralProductionBonus > 0)
            bonusParts.Add($"+{GeneralProductionBonus} to all native production");
        foreach (var kvp in ResourceProductionBonus)
            bonusParts.Add($"+{kvp.Value} {kvp.Key.GetNestedTooltipLink()}");
        string bonusText = string.Join(" and ", bonusParts);

        // 2) Build the “criteria” part
        string criteriaText;
        if (EffectCriteria_TagsAny.Count > 0)
        {
            // “for Farm or Barn”
            var links = EffectCriteria_TagsAny.Select(t => t.GetNestedTooltipLink());
            criteriaText = $"to {string.Join(" or ", links)} objects";
        }
        else // EffectCriteria_TagsAll.Count > 0
        {
            // “for each adjacent object with all tags Grain and Flour”
            var links = EffectCriteria_TagsAll.Select(t => t.GetNestedTooltipLink());
            criteriaText = $"to {string.Join(" ", links)} objects";
        }

        return $"{bonusText} {criteriaText}.";
    }
}
