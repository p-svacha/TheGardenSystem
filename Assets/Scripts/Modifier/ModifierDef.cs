using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A modifier is a modular ObjectEffect that can be attached to any tile or object, to change its behaviour or production.
/// </summary>
public class ModifierDef : Def, ITooltipTarget
{
    public string SpritePath => "Sprites/ObjectModifiers/" + DefName;

    public override Sprite Sprite => ResourceManager.LoadSprite(SpritePath);

    /// <summary>
    /// The effect that may get applied to the object holding the modifier.
    /// </summary>
    public ObjectEffect Effect { get; init; }

    /// <summary>
    /// Flag if the same object can get this modifier multiple times.
    /// </summary>
    public bool IsStackable { get; init; }

    public override void ResolveReferences()
    {
        Effect.EffectSource = this;
    }

    public override bool Validate()
    {
        if (!Effect.Validate(out string invalidReason)) ThrowValidationError($"ObjectModifierDef {DefName} has an invalid Effect. Reason: {invalidReason}");

        return base.Validate();
    }


    #region ITooltipTarget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetTooltipBodyText(List<ITooltipTarget> dynamicReferences)
    {
        string desc = Description;
        desc += $"\n\n{Effect.GetDescription()}";

        return desc;
    }

    public string NestedTooltipLinkId => $"ObjectModifierDef_{DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => TooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
