using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An ObjectEffect defines a specific effect originating from a source tile, that affects the resource production of tiles or objects.
/// </summary>
public abstract class ObjectEffect
{
    /// <summary>
    /// Returns if this is a valid ObjectEffect, including a reason of why if it is not.
    /// </summary>
    public abstract bool Validate(out string invalidReason);

    /// <summary>
    /// Applies all resource production modifiers that originate from this effect.
    /// </summary>
    public abstract void ApplyEffect(MapTile sourceTile, Dictionary<MapTile, Dictionary<ResourceDef, ResourceProduction>> tileProductions);

    /// <summary>
    /// Returns what this effect does as a human-readable string including TMPro tooltip links.
    /// </summary>
    public abstract string GetDescription();

    /// <summary>
    /// Returns the generalized "+1 to all native production" "-2 to food and fiber production" string.
    /// </summary>
    protected string GetBonusPartOfDescription(int nativeProductionModifier, Dictionary<ResourceDef, int> resourceProductionModifier)
    {
        string desc = "";
        if (nativeProductionModifier != 0)
        {
            string sign = nativeProductionModifier > 0 ? "+" : "";
            desc += $"<nobr>{sign}{nativeProductionModifier} native production, </nobr>";
        }
        foreach (var kvp in resourceProductionModifier)
        {
            string sign = kvp.Value > 0 ? "+" : "";
            desc += $"<nobr>{sign}{kvp.Value} {kvp.Key.GetNestedTooltipLink()} production, </nobr>";
        }

        // Remove trailing ", </nobr>" if present
        const string trailing = ", </nobr>";
        if (desc.EndsWith(trailing))
        {
            desc = desc.Substring(0, desc.Length - trailing.Length) + "</nobr>";
        }

        return desc;
    }

    /// <summary>
    /// Returns the generalized "plant or flower object", "crafting structure object", "honey producing object" string.
    /// </summary>
    protected string GetCriteriaPartDescription(List<ObjectTagDef> tagsAny, List<ObjectTagDef> tagsAll, List<ResourceDef> nativeProdAny, bool plural = false)
    {
        string desc = "";

        string objString = plural ? "objects" : "object";
        if (tagsAny.Count > 0)
        {
            var links = tagsAny.Select(t => t.GetNestedTooltipLink());
            desc += $"<nobr>{string.Join(" or ", links)} {objString}, </nobr>";
        }
        if (tagsAll.Count > 0)
        {
            var links = tagsAll.Select(t => t.GetNestedTooltipLink());
            desc += $"<nobr>{string.Join(" ", links)} {objString}, </nobr>";
        }
        if (nativeProdAny.Count > 0)
        {
            var links = nativeProdAny.Select(r => r.GetNestedTooltipLink());
            desc += $"<nobr>{string.Join(" or ", links)} producing {objString}, </nobr>";
        }

        // Remove trailing ", </nobr>" if present
        const string trailing = ", </nobr>";
        if (desc.EndsWith(trailing))
        {
            desc = desc.Substring(0, desc.Length - trailing.Length) + "</nobr>";
        }

        return desc;
    }
}
