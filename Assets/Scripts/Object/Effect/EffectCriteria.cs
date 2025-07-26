using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The criteria of an effect defining under which circumstances it is applied.
/// </summary>
public class EffectCriteria
{
    /// <summary>
    /// If not empty, the effect is only applied when the object in question has any of these tags.
    /// </summary>
    public List<ObjectTagDef> TagsAny { get; init; } = new();

    /// <summary>
    /// If not empty, the effect is only applied when the object in question has all of these tags.
    /// </summary>
    public List<ObjectTagDef> TagsAll { get; init; } = new();

    /// <summary>
    /// If not empty, the effect is only applied when the object in question produces a resource in this list natively.
    /// </summary>
    public List<ResourceDef> NativeProductionAny { get; init; } = new();

    /// <summary>
    /// If not empty, the effect is only applied when the terrain in question matches one of the terrains in this list.
    /// </summary>
    public List<TerrainDef> TerrainAny { get; init; } = new();

    /// <summary>
    /// Checks if this criteria is valid the way it is defined.
    /// </summary>
    public bool Validate(out string invalidReason)
    {
        // Null checks
        if (TagsAny.Any(t => t == null))
        {
            invalidReason = "TagsAny contains a tag that is null.";
            return false;
        }
        if (TagsAll.Any(t => t == null))
        {
            invalidReason = "TagsAll contains a tag that is null.";
            return false;
        }
        if (NativeProductionAny.Any(t => t == null))
        {
            invalidReason = "NativeProductionAny contains a resource that is null.";
            return false;
        }
        if (TerrainAny.Any(t => t == null))
        {
            invalidReason = "TerrainAny contains a terrain that is null.";
            return false;
        }

        // Other checks
        if (TagsAny.Count == 0 && TagsAll.Count == 0 && NativeProductionAny.Count == 0 && TerrainAny.Count == 0)
        {
            invalidReason = "There is no criteria defined for when the effect should be triggered.";
            return false;
        }
        if (TagsAny.Count > 0 && TagsAll.Count > 0)
        {
            invalidReason = "There is conflicting tag criteria defined for when this effect should be triggered.";
            return false;
        }

        invalidReason = "";
        return true;
    }

    /// <summary>
    /// Returns if the given tile fulfills this effect criteria.
    /// </summary>
    public bool IsFulfilledOn(MapTile tile)
    {
        if (TerrainAny.Count > 0 && !TerrainAny.Contains(tile.Terrain.Def)) return false;

        if (!tile.HasObject)
        {
            if (TagsAny.Count > 0 || TagsAll.Count > 0 || NativeProductionAny.Count > 0) return false;
        }

        else if (TagsAny.Count > 0 && (!tile.Object.HasAnyOfTags(TagsAny))) return false;
        else if (TagsAll.Count > 0 && (!tile.Object.HasAllTags(TagsAll))) return false;
        else if (NativeProductionAny.Count > 0 && (!tile.Object.ProducesAnyOfResourcesNatively(NativeProductionAny))) return false;

        return true;
    }

    /// <summary>
    /// Returns the generalized "plant or flower object", "crafting structure object", "honey producing object" string.
    /// </summary>
    public string GetAsReadableString(bool includeObjectLiteral = true, bool plural = false)
    {
        string desc = "";

        string objString = "";
        if (includeObjectLiteral)
        {
            if (plural) objString = "objects";
            else objString = "object";
        }
        if (TagsAny.Count > 0)
        {
            var links = TagsAny.Select(t => t.GetTooltipLink());
            desc += $"<nobr>{string.Join(" or ", links)} {objString}, </nobr>";
        }
        if (TagsAll.Count > 0)
        {
            var links = TagsAll.Select(t => t.GetTooltipLink());
            desc += $"<nobr>{string.Join(" ", links)} {objString}, </nobr>";
        }
        if (NativeProductionAny.Count > 0)
        {
            var links = NativeProductionAny.Select(r => r.GetTooltipLink());
            desc += $"<nobr>{string.Join(" or ", links)} producing {objString}, </nobr>";
        }
        if (TerrainAny.Count > 0)
        {
            string prefix = "";
            if (includeObjectLiteral && desc == "") prefix = $"{objString} ";
            var links = TerrainAny.Select(r => r.GetTooltipLink());
            desc += $"<nobr>{prefix}on {string.Join(" or ", links)} terrain, </nobr>";
        }

        // Remove trailing ", </nobr>" if present
        const string trailing = ", </nobr>";
        if (desc.EndsWith(trailing))
        {
            desc = desc.Substring(0, desc.Length - trailing.Length) + "</nobr>";
        }

        return desc;
    }

    public EffectCriteria GetCopy()
    {
        return new EffectCriteria()
        {
            TagsAny = new List<ObjectTagDef>(this.TagsAny),
            TagsAll = new List<ObjectTagDef>(this.TagsAll),
            NativeProductionAny = new List<ResourceDef>(this.NativeProductionAny),
            TerrainAny = new List<TerrainDef>(this.TerrainAny),
        };
    }
}
