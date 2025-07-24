using UnityEngine;
using System.Collections.Generic;

public class ObjectDef : Def, IDraftable
{
    public string FlavorText { get; init; } = "";
    public ObjectTierDef Tier { get; init; } = null;
    public List<ObjectTagDef> Tags { get; init; } = new();
    public ResourceCollection BaseResources { get; init; } = new();
    public List<ObjectEffect> Effects { get; init; } = new();
    new public Sprite Sprite => ResourceManager.LoadSprite("Sprites/Objects/" + DefName);

    public override bool Validate()
    {
        if (Tier == null) throw new System.Exception($"Tier must be set of {DefName}.");
        foreach (ObjectEffect effect in Effects)
        {
            if (!effect.Validate(out string invalidReason)) throw new System.Exception($"TerrainDef {DefName} has an invalid Effect: {invalidReason}");
        }

        return true;
    }

    #region Getters

    /// <summary>
    /// Returns all effects of this ObjectDef as a human-readable string including TMPro tooltip links.
    /// </summary>
    public string GetEffectDescription()
    {
        string desc = "";

        foreach (ObjectEffect effect in Effects)
        {
            desc += effect.GetDescription() + "\n";
        }
        desc = desc.TrimEnd('\n');

        return desc;
    }

    /// <summary>
    /// Returns all tags of this ObjectDef as a human-readable string including TMPro tooltip links.
    /// </summary>
    public string GetTagsAsLinkedString()
    {
        string tags = "";
        foreach (ObjectTagDef tag in Tags) tags += $"{tag.GetNestedTooltipLink()}   ";
        tags = tags.TrimEnd(' ');

        return tags;
    }

    #endregion
}
