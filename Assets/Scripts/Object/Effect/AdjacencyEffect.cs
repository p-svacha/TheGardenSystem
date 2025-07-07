using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specific kind of of ObjectEffect that gets applied to all adjacent objects of a source object. 
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
    /// The amount of production that gets to added all resources an object produces natively (according to Object.GetBaseResourceProduction)
    /// </summary>
    public int GeneralProductionBonus { get; init; } = 0;

    /// <summary>
    /// The amount of production that gets to added to specific resources, disregarding what resources the object produces natively.
    /// </summary>
    public Dictionary<ResourceDef, int> ResourceProductionBonus { get; init; } = new();

    public override void ApplyEffect(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions)
    {
        foreach(MapTile tile in sourceTile.GetAdjacentTiles())
        {
            // Check if effect can be applied
            bool doApplyEffect = true;
            if (AffectedTagsAny.Count > 0 && (!tile.HasObject || !tile.Object.HasAnyOfTags(AffectedTagsAny))) doApplyEffect = false;
            if (AffectedTagsAll.Count > 0 && (!tile.HasObject || !tile.Object.HasAllTags(AffectedTagsAll))) doApplyEffect = false;
            if (!doApplyEffect) continue;

            // Apply bonus to native resources
            if (GeneralProductionBonus > 0)
            {
                foreach (ResourceDef resource in tile.Object.GetBaseResourceProduction().Keys)
                {
                    ProductionModifier modifier = new ProductionModifier(sourceTile.Object.LabelCap, ProductionModifierType.Additive, GeneralProductionBonus);
                    tileProductions[tile][resource].AddModifier(modifier);
                }
            }

            // Apply bonus to specific resources
            foreach (var kvp in ResourceProductionBonus)
            {
                ResourceDef resource = kvp.Key;
                int productionBonus = kvp.Value;

                ProductionModifier modifier = new ProductionModifier(sourceTile.Object.LabelCap, ProductionModifierType.Additive, productionBonus);
                tileProductions[tile][resource].AddModifier(modifier);
            }
        }
    }
}
