using UnityEngine;
using System.Collections.Generic;

public class ObjectDef : Def, IDraftable, ITooltipTarget
{
    public ObjectTierDef Tier { get; init; } = null;
    public List<ObjectTagDef> Tags { get; init; } = new();
    public Dictionary<ResourceDef, int> NativeProduction { private get; init; } = new();
    public List<ObjectEffect> Effects { get; init; } = new();

    private Sprite _Sprite;
    new public Sprite Sprite => _Sprite;

    private ResourceCollection _NativeProduction;
    public ResourceCollection GetNativeProduction() => _NativeProduction;

    public override bool Validate()
    {
        if (Tier == null) throw new System.Exception($"Tier must be set of {DefName}.");
        foreach (ObjectEffect effect in Effects)
        {
            if (!effect.Validate(out string invalidReason)) ThrowValidationError($"ObjectDef {DefName} has an invalid Effect. Reason: {invalidReason}");
        }

        return true;
    }

    public override void ResolveReferences()
    {
        _Sprite = ResourceManager.LoadSprite("Sprites/Objects/" + DefName);
        _NativeProduction = new ResourceCollection(NativeProduction);
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
        foreach (ObjectTagDef tag in Tags) tags += $"{tag.GetTooltipLink()}   ";
        tags = tags.TrimEnd(' ');

        return tags;
    }

    #endregion

    #region ITooltipTarget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetTooltipBodyText(List<ITooltipTarget> dynamicReferences)
    {
        // Tags
        string tags = "";
        foreach (ObjectTagDef tag in Tags)
        {
            tags += $"{tag.GetTooltipLink()} ";
        }
        tags = tags.TrimEnd(' ');

        // Native production
        string nativeProd = "";
        if (!GetNativeProduction().IsEmpty)
        {
            nativeProd += $"\n\nNative Production: {GetNativeProduction().GetAsSingleLinkedString()}";
        }

        // Effects
        string effectDescriptions = "";
        List<ObjectEffect> effects = Effects;
        if (effects.Count > 0)
        {
            effectDescriptions += "\n";
            foreach (ObjectEffect effect in effects) effectDescriptions += $"\n <color=#555555>•</color> {effect.GetDescription()}";
        }

        return $"{tags}\n\n<color=#999999>{Description}</color>{nativeProd}{effectDescriptions}";
    }

    public string NestedTooltipLinkId => $"ObjectDef_{DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => TooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
