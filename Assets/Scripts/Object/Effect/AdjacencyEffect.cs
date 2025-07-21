using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to adjacent objects of a source tile. 
/// </summary>
public class AdjacencyEffect : ObjectEffect
{
    /// <summary>
    /// If not empty, only objects with a tag included in this list are affected by this effect.
    /// </summary>
    public List<ObjectTagDef> AffectedTagsAny { get; init; } = new();

    /// <summary>
    /// If not empty, only objects that have all tags in this list are affected by this effect.
    /// </summary>
    public List<ObjectTagDef> AffectedTagsAll { get; init; } = new();

    /// <summary>
    /// The amount of production that gets to added to all resources an object produces natively (according to Object.GetBaseResourceProduction)
    /// </summary>
    public int GeneralProductionBonus { get; init; } = 0;

    /// <summary>
    /// The amount of production that gets to added to specific resources, disregarding what resources the object produces natively.
    /// </summary>
    public Dictionary<ResourceDef, int> ResourceProductionBonus { get; init; } = new();

    public override bool Validate(out string invalidReason)
    {
        if (AffectedTagsAny.Any(t => t == null))
        {
            invalidReason = "AffectedTagsAny contains a tag that is null.";
            return false;
        }
        if (AffectedTagsAll.Any(t => t == null))
        {
            invalidReason = "AffectedTagsAll contains a tag that is null.";
            return false;
        }

        if (AffectedTagsAny.Count == 0 && AffectedTagsAll.Count == 0)
        {
            invalidReason = "There is no criteria defined for when this effect should be triggered.";
            return false;
        }
        if (AffectedTagsAny.Count > 0 && AffectedTagsAll.Count > 0)
        {
            invalidReason = "There is conflicting criteria defined for when this effect should be triggered.";
            return false;
        }

        invalidReason = "";
        return true;
    }

    public override void ApplyEffect(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        foreach(MapTile tile in sourceTile.GetAdjacentTiles())
        {
            // Check if effect can be applied
            bool doApplyEffect = true;

            if (!tile.HasObject) doApplyEffect = false;
            else if (AffectedTagsAny.Count > 0 && (!tile.Object.HasAnyOfTags(AffectedTagsAny))) doApplyEffect = false;
            else if (AffectedTagsAll.Count > 0 && (!tile.Object.HasAllTags(AffectedTagsAll))) doApplyEffect = false;

            if (!doApplyEffect) continue;

            // Apply bonus to native resources
            if (GeneralProductionBonus > 0)
            {
                foreach (ResourceDef resource in tile.Object.NativeResources)
                {
                    ProductionModifier modifier = new ProductionModifier(source: sourceTile.Object, ProductionModifierType.Additive, GeneralProductionBonus);
                    tileProductions[tile][resource].AddModifier(modifier);
                }
            }

            // Apply bonus to specific resources
            foreach (var kvp in ResourceProductionBonus)
            {
                ResourceDef resource = kvp.Key;
                int productionBonus = kvp.Value;

                ProductionModifier modifier = new ProductionModifier(source: sourceTile.Object, ProductionModifierType.Additive, productionBonus);
                tileProductions[tile][resource].AddModifier(modifier);
            }
        }
    }

    public override string GetDescription()
    {
        // 1) Build the bonus part
        var bonusParts = new List<string>();
        if (GeneralProductionBonus > 0)
            bonusParts.Add($"+{GeneralProductionBonus} native production");
        foreach (var kvp in ResourceProductionBonus)
            bonusParts.Add($"+{kvp.Value} {kvp.Key.GetNestedTooltipLink()}");
        string bonusText = string.Join(" and ", bonusParts);

        // 2) Build the “affected” criteria part
        string affectedText;
        if (AffectedTagsAny.Count > 0)
        {
            // “for each adjacent Farm or Barn”
            var links = AffectedTagsAny.Select(t => t.GetNestedTooltipLink());
            affectedText = $"to each adjacent {string.Join(" or ", links)} objects";
        }
        else // AffectedTagsAll.Count > 0
        {
            // “for each adjacent object with all tags Grain and Flour”
            var links = AffectedTagsAll.Select(t => t.GetNestedTooltipLink());
            affectedText = $"to each adjacent {string.Join(" ", links)} objects";
        }

        return $"{bonusText} {affectedText}.";
    }
}
